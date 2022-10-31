using System;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;
using System.IO;
using static pcr_rank_equipment_query.Load;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using System.Data.SQLite;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace pcr_rank_equipment_query
{
    public partial class MainForm : Form
    {
        private System.Timers.Timer delayTimer = new System.Timers.Timer(800) { Enabled = true };
        private System.Timers.Timer TimerEquipment = new System.Timers.Timer(800) { Enabled = true };
        private System.Timers.Timer TimerEquipmentFiltering = new System.Timers.Timer(800) { Enabled = true };
        private System.Timers.Timer TimerquestFiltering = new System.Timers.Timer(800) { Enabled = true };
        public bool enableFiltering = false;
        public bool equipmentFiltering = false;
        public bool equipmentFilteringFiltering = false;
        public bool questFiltering = false;
        List<equipment_data_View> equipment_data_visible = new List<equipment_data_View>();
        List<Map_data> map_wave_data_visible = new List<Map_data>();

        List<quest_data_List> quest_data_visible = new List<quest_data_List>();
        List<quest_data_List> resultList = new List<quest_data_List>();

        List<int> reward_id_list = new List<int>();
        private int sortColumn = -1;
        private bool reverseSort;

        public MainForm()
        {
            InitializeComponent();
            /*string[] f = File.ReadAllLines(@"C:\Users\libaibaba\source\repos\pcr_rank_equipment_query\新建文本文档.txt");
            List<string> s = new List<string>(f);
            s=s.Distinct().ToList();
            string[] g = s.ToArray();
            File.WriteAllLines(@"C:\Users\libaibaba\source\repos\pcr_rank_equipment_query\新建文本文档1.txt", g);
            */
            

            //MessageBox.Show(""+(11034121 % 100000)/1000);
            BuildAssetStructures();
        }
        private async void BuildAssetStructures()
        {
            foreach (ColumnHeader ch in listView1.Columns)
            {
                ch.Width = listView1.Width / listView1.Columns.Count;
            }
            /*foreach (ColumnHeader ch in listView3.Columns)
            {
                ch.Width = listView3.Width / listView3.Columns.Count;
            }*/
            /*foreach (ColumnHeader ch in listView4.Columns)
            {
                ch.Width = listView4.Width / listView4.Columns.Count;
            }*/
            /*foreach (ColumnHeader ch in listView2.Columns)
            {
                ch.Width = listView2.Width / listView2.Columns.Count;
            }*/
            listView2.Columns[0].Width = 60;
            listView2.Columns[1].Width = 172;
            listView2.Columns[2].Width = 48;
            listView2.Columns[3].Width = 48;

            listView3.Columns[0].Width = 118;
            listView3.Columns[1].Width = 60;
            listView3.Columns[2].Width = 60;

            await Task.Run(() => SaveDB());
            listView1.VirtualListSize = unit_datavisible.Count;
            int comboBox1i = Convert.ToInt32(comboBox1.Text) + 1;
            while (comboBox1.Items.Count < rank)
            {
                comboBox1.Items.Add(comboBox1i);
                comboBox1i++;
            }
            
            //MessageBox.Show(unit_datavisible[0].rank+"");
            LoadAlleq();
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            delayTimer.Elapsed += new ElapsedEventHandler(DelayTimer_Elapsed);
            TimerEquipment.Elapsed += new ElapsedEventHandler(TimerEquipment_Elapsed);
            TimerEquipmentFiltering.Elapsed += new ElapsedEventHandler(TimerEquipmentFiltering_Elapsed);
            TimerquestFiltering.Elapsed += new ElapsedEventHandler(TimerquestFiltering_Elapsed);
        }

        private void TextBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                enableFiltering = false;
                textBox1.Text = "输入角色查询(多个请用;分开)";
                textBox1.ForeColor = SystemColors.GrayText;
                FilterCharacter();
            }
        }
        private void TextBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "输入角色查询(多个请用;分开)")
            {
                textBox1.Text = "";
                textBox1.ForeColor = SystemColors.WindowText;
                enableFiltering = true;
            }
        }
        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            if (enableFiltering)
            {
                if (delayTimer.Enabled)
                {
                    delayTimer.Stop();
                    delayTimer.Start();
                }
                else
                {
                    delayTimer.Start();//fin
                }
            }
        }
        private void DelayTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            delayTimer.Stop();
            Invoke(new Action(FilterCharacter));
        }
        void FilterCharacter()
        {
            comboBox2.Text = rank.ToString();
            int comboBox2i = rank;
            while (comboBox2.Items.Count < rank)
            {
                comboBox2.Items.Add(comboBox2i);
                comboBox2i--;
            }
            listView1.BeginUpdate();
            listView1.SelectedIndices.Clear();
            unit_datavisible = unit_data_list;

            if (textBox1.Text != "输入角色查询(多个请用;分开)")
            {
                string[] Array = textBox1.Text.Split(';', '；');
                List<unit_data> temp = new List<unit_data>();

                foreach (var a in Array)
                    if (!string.IsNullOrEmpty(a))
                    {
                        foreach (var unit in unit_datavisible)
                        {
                            if (unit.SubItems[0].Text.IndexOf(a, StringComparison.OrdinalIgnoreCase) >= 0 |
                                unit.SubItems[1].Text.IndexOf(a) >= 0)
                            {
                                temp.Add(unit);
                            }
                        }
                    }

                /*unit_datavisible = unit_datavisible.FindAll(
                        x => x.SubItems[0].Text.IndexOf(textBox1.Text, StringComparison.OrdinalIgnoreCase) >= 0 |
                        x.SubItems[1].Text.IndexOf(textBox1.Text) >= 0);*/
                unit_datavisible = temp;
            }
            listView1.VirtualListSize = unit_datavisible.Count;
            listView1.EndUpdate();
        }
        private void TextBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "输入武器查询(多个请用;分开)")
            {
                textBox2.Text = "";
                textBox2.ForeColor = SystemColors.WindowText;
                equipmentFiltering = true;
            }
        }
        private void TextBox2_TextChanged(object sender, EventArgs e)
        {
            if (equipmentFiltering)
            {
                if (TimerEquipment.Enabled)
                {
                    TimerEquipment.Stop();
                    TimerEquipment.Start();
                }
                else
                {
                    TimerEquipment.Start();//fin
                }
            }
        }
        private void TextBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                equipmentFiltering = false;
                textBox2.Text = "输入武器查询(多个请用;分开)";
                textBox2.ForeColor = SystemColors.GrayText;

                LoadAlleq();
            }
        }
        private void TimerEquipment_Elapsed(object sender, ElapsedEventArgs e)
        {
            TimerEquipment.Stop();
            Invoke(new Action(FilterEquipment));
        }
        void FilterEquipment()
        {
            listView4.BeginUpdate();
            listView4.SelectedIndices.Clear();

            if (textBox2.Text != "输入武器查询(多个请用;分开)")
            {
                listView1.BeginUpdate();
                listView1.SelectedIndices.Clear();
                listView1.EndUpdate();
                var ac = Regex.Replace(textBox2.Text, @"[0-9]", "");

                equipment_data_visible = new List<equipment_data_View>();

                string[] Array = ac.Split(';', '；');
                foreach (var a in Array)
                    if (!string.IsNullOrEmpty(a))
                    {
                        ListView4Array(a);
                        //MessageBox.Show(a);
                    }
                foreach (var a in equipment_data_visible)
                {
                    //MessageBox.Show(a.equipment_id + "\n" + a.equipment_name);
                    a.SetSubItems();
                }
            }
            listView4.VirtualListSize = equipment_data_visible.Count;
            listView4.EndUpdate();
        }

        void ListView4Array(string f)
        {
            var FindAllequipment = equipment_data_list.FindAll(x => x.equipment_name.IndexOf(f) >= 0);
            foreach (var adde in FindAllequipment)
            {
                int fie3 = 0;
                kucun_data_list.ForEach(k => {
                    if (k.field1 % 10000 == adde.equipment_id % 10000)
                    {
                        fie3 = k.field3;
                    }
                });
                /*var haixu = kucun_data_list.FirstOrDefault(x => x.field1 % 10000 == adde.equipment_id % 10000);
                if (haixu == null)
                {
                    //MessageBox.Show(eq.equipment_id + "");
                    continue;
                }*/
                equipment_data_visible.Add(new equipment_data_View()
                {
                    equipment_id = adde.equipment_id,
                    equipment_name = adde.equipment_name,
                    consume_num_1 = 1,
                    haixu = fie3
                });
            }
        }

        private void ListView1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            //MessageBox.Show("d"+ e.ItemIndex);
            unit_datavisible = unit_datavisible.Distinct().ToList();
            try
            {
                e.Item = unit_datavisible[e.ItemIndex];
            }
            catch
            {
                MessageBox.Show(unit_datavisible.Count + "\nlistView1_RetrieveVirtualItem");
            }

        }

        private void ListView4_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            equipment_data_visible = equipment_data_visible.Distinct().ToList();
            try
            {
                e.Item = equipment_data_visible[e.ItemIndex];
            }
            catch
            {
                MessageBox.Show(equipment_data_visible.Count + "\nlistView4_RetrieveVirtualItem");
            }
        }

        private void ListView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            textBox2.Text = "输入武器查询(多个请用;分开)";
            textBox2.ForeColor = SystemColors.GrayText;

            listView4.BeginUpdate();
            listView4.SelectedIndices.Clear();

            equipment_data_visible = new List<equipment_data_View>();

            //unit_data item = (unit_data)e.Item;

            List<unit_data> u = new List<unit_data>();

            if (listView1.SelectedIndices.Count >= 1)
            {

                foreach (var v in listView1.SelectedIndices)
                {
                    unit_data selectasset = (unit_data)listView1.Items[Convert.ToInt32(v.ToString())];
                    //MessageBox.Show(selectasset.equipment_id + "");
                    u.Add(selectasset);
                }
            }

            foreach (var uu in u)
            {
                equipment_data_visible = Listview1(uu, int.Parse(comboBox1.Text), int.Parse(comboBox2.Text));
            }
            //List<equipment_data_View> temp = listview1(item, int.Parse(comboBox1.Text), int.Parse(comboBox2.Text));
            //equipment_data_visible = listview1(item, item.rank, int.Parse(comboBox2.Text));


            equipment_data_visible.Sort((a, b) =>
            {
                var asf = a.haixu;
                var bsf = b.haixu;
                return reverseSort ? asf.CompareTo(bsf) : bsf.CompareTo(asf);
            });

            foreach (var v in equipment_data_visible)
            {
                v.SetSubItems();
            }

            listView4.VirtualListSize = equipment_data_visible.Count;
            listView4.EndUpdate();
        }


        List<equipment_data_View> Listview1(unit_data e, int minr, int maxr) {
            Listv1(true);

            Dictionary<int, int[]>keyValuePairs = new Dictionary<int, int[]>();
            for (var i= minr;i<= maxr;i++)
            {
                var fin = unit_promotion_list.Find(x => x.unit_id.Equals(e.unit_id)&x.promotion_level.Equals(i));

                keyValuePairs.Add(i,new int[] { 
                    fin.equip_slot_1,
                    fin.equip_slot_2,
                    fin.equip_slot_3,
                    fin.equip_slot_4,
                    fin.equip_slot_5,
                    fin.equip_slot_6,
                });
            }

            for (var i = minr; i < maxr; i++)
            {
                Addequipment_data_View(keyValuePairs[i][0]);
                Addequipment_data_View(keyValuePairs[i][1]);
                Addequipment_data_View(keyValuePairs[i][2]);
                Addequipment_data_View(keyValuePairs[i][3]);
                Addequipment_data_View(keyValuePairs[i][4]);
                Addequipment_data_View(keyValuePairs[i][5]);
            }
            

            if (e.equip_slot1 != 1)
            {
                Addequipment_data_View(keyValuePairs[maxr][0]);
            }
            if (e.equip_slot2 != 1)
            {
                Addequipment_data_View(keyValuePairs[maxr][1]);
            }
            if (e.equip_slot3 != 1)
            {
                Addequipment_data_View(keyValuePairs[maxr][2]);
            }
            if (e.equip_slot4 != 1)
            {
                Addequipment_data_View(keyValuePairs[maxr][3]);
            }
            if (e.equip_slot5 != 1)
            {
                Addequipment_data_View(keyValuePairs[maxr][4]);
            }
            if (e.equip_slot6 != 1)
            {
                Addequipment_data_View(keyValuePairs[maxr][5]);
            }


            var equipment_data_visibletemp = new List<equipment_data_View>();

            foreach (var v in equipment_data_visible)
            {
                
                var temp = equipment_data_visibletemp.Find(x => (x.equipment_id % 10000).Equals(v.equipment_id % 10000));
                if (temp == null)
                {
                    equipment_data_visibletemp.Add(v);
                }
                else
                {
                    temp.consume_num_1 += v.consume_num_1;
                }
            }

            foreach (var v in equipment_data_visibletemp)
            {
                var l = kucun_data_list.Find(x => x.field1.Equals(v.equipment_id) | (x.field1 % 10000).Equals(v.equipment_id % 10000));
                if (l != null)
                {
                    var haixu = v.consume_num_1 - l.field3;
                    if (haixu > 0)
                    {
                        v.haixu = haixu;
                        v.BackColor = ColorTranslator.FromHtml("#a3f1ff");
                    }
                    else
                    {
                        v.haixu = 0;
                        v.BackColor = ColorTranslator.FromHtml("#ffffff");
                    }
                }
                else
                {
                    v.haixu = v.consume_num_1;
                    v.BackColor = ColorTranslator.FromHtml("#a3f1ff");
                    //MessageBox.Show(v.equipment_id+"");
                }
                if (v.haixu != 0)
                {
                    v.jixu = v.consume_num_1 - v.haixu;
                    //MessageBox.Show(v.equipment_name + "");
                }
                else
                {
                    v.jixu = 0;
                }
            }
            return equipment_data_visibletemp;
        }

        public void Addequipment_data_View(int equipment_id)
        {
            var v = equipment_craft_list.Find(
            x => (x.equipment_id % 10000).Equals(equipment_id % 10000));

            if (v != null)
            {
                /*MessageBox.Show(
                    equipment_id + "\n"
                    );*/
                var ind = Indexfor(equipment_id);
                if (ind != null)
                {
                    //if (equipment_id==105107) { MessageBox.Show("105107"); }
                    equipment_data_visible.Add(ind);
                }


                if (v.condition_equipment_id_2 != 0)
                {
                    Addequipment_data_View(v.condition_equipment_id_2);
                }
                if (v.condition_equipment_id_3 != 0)
                {
                    Addequipment_data_View(v.condition_equipment_id_3);
                }
                if (v.condition_equipment_id_4 != 0)
                {
                    Addequipment_data_View(v.condition_equipment_id_4);
                }
            }
            else
            {
                equipment_data_visible.Add(new equipment_data_View()
                {
                    equipment_id = equipment_id,

                    equipment_name = equipment_data_list.Find(
                        x => x.equipment_id == equipment_id).equipment_name,

                    consume_num_1 = 1
                });
            }
        }
        internal equipment_data_View Indexfor(int equipment_id)
        {
            var equipment_name = equipment_data_list.Find(
            x => (x.equipment_id % 10000).Equals(equipment_id % 10000));

            var consume_num_1 = equipment_craft_list.Find(
            x => (x.equipment_id % 10000).Equals(equipment_id % 10000));

            if (equipment_name != null && consume_num_1 != null)
            {
                equipment_data_View v = new equipment_data_View()
                {
                    equipment_id = equipment_id,

                    equipment_name = equipment_name.equipment_name,

                    consume_num_1 = consume_num_1.consume_num_1

                };
                return v;
            }
            else
            {
                //MessageBox.Show(equipment_id+"");
                return null;
            }
            /*MessageBox.Show(equipment_idid + "\n" +
                  equipment_namename + "\n" +
                 consume_num_111
                  );*/
        }

        void FilterQuest(int r)
        {
            quest_data_lists.ForEach(q =>
            {
                q.wave_group_data_list.ForEach(w =>
                {
                    w.enemy_reward_data_list.ForEach(en =>
                    {
                        if (en.reward_id % 10000 == r % 10000)
                        {
                            #region DeBug
                            /*if (q.odds != en.odds)
                            {
                                MessageBox.Show(q.quest_id+"\n"+
                                    q.quest_name + "\n" +
                                    //w.wave_group_id +"\n" +
                                    //en.drop_reward_id+"\n"+
                                    //en.reward_id % 10000 + "\n" +
                                    //en.reward_id + "\n" +
                                    q.odds + "\n" +
                                    en.odds + "\n");
                            }*/
                            #endregion
                            q.odds = en.odds;
                            //MessageBox.Show(q.odds +" "+ en.odds);
                            quest_data_visible.Add(q);
                        }
                    });
                });
            });
        }

        private void ListView2_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = resultList[e.ItemIndex];
        }

        private void ListView2_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (sortColumn != e.Column)
            {
                reverseSort = false;
            }
            else
            {
                reverseSort = !reverseSort;
            }
            sortColumn = e.Column;
            listView2.BeginUpdate();
            listView2.SelectedIndices.Clear();
            if (sortColumn == 3)
            {
                //MessageBox.Show("?");
                resultList.Sort((a, b) =>
                {
                    var asf = a.count;
                    var bsf = b.count;
                    return reverseSort ? bsf.CompareTo(asf) : asf.CompareTo(bsf);
                });
            }
            listView2.EndUpdate();
        }


        int Frequency(List<quest_data_List> c, quest_data_List o)
        {
            int result = 0;
            if (o == null)
            {
                foreach (var e in c)
                {
                    if (e == null)
                    {
                        result++;
                    }
                }
            }
            else
            {
                foreach (var e in c)
                {
                    if (o.Equals(e))
                    {
                        result++;
                    }
                }
            }
            //MessageBox.Show(result+"");
            return result;
        }

        void LoadAlleq()
        {

            listView4.BeginUpdate();
            listView4.SelectedIndices.Clear();

            Listv1(false);

            equipment_data_visible = new List<equipment_data_View>();
            foreach (var eq in equipment_data_list)
            {
                int fie3 = 0;
                kucun_data_list.ForEach(k => {
                    if (k.field1 % 10000 == eq.equipment_id % 10000)
                    {
                        fie3 = k.field3;
                    }
                });
                /*var haixu = kucun_data_list.FirstOrDefault(x => x.field1 % 10000 == eq.equipment_id % 10000);
                if (haixu == null)
                {
                    //MessageBox.Show(eq.equipment_id + "");
                    continue;
                }*/
                equipment_data_visible.Add(new equipment_data_View()
                {
                    equipment_id = eq.equipment_id,
                    equipment_name = equipment_data_list.Find(
                        x => x.equipment_id == eq.equipment_id).equipment_name,
                    consume_num_1 = 1,
                    haixu = fie3
                });
            }

            equipment_data_visible.Sort((a, b) =>
            {
                var asf = a.haixu;
                var bsf = b.haixu;
                return reverseSort ? bsf.CompareTo(asf) : asf.CompareTo(bsf);
            });

            foreach (var a in equipment_data_visible)
            {
                a.SetSubItems();
            }

            listView4.VirtualListSize = equipment_data_visible.Count;
            listView4.EndUpdate();
        }

        void Listv1(bool cha)
        {
            if (cha)
            {
                listView4.Columns[2].Text = "还需";
            }
            else
            {
                listView4.Columns[2].Text = "库存剩余";
            }
        }

        private void ListView3_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = map_wave_data_visible[e.ItemIndex];
        }

        private void ListView2_MouseClick(object sender, MouseEventArgs e)
        {
            listView3.BeginUpdate();
            listView3.SelectedIndices.Clear();

            if (e.Button == MouseButtons.Left | e.Button == MouseButtons.Right && listView2.SelectedIndices.Count > 0)
            {
                quest_data_List selectasset = (quest_data_List)listView2.Items[Convert.ToInt32(listView2.SelectedIndices[0])];
                map_wave_data_visible = selectasset.map_wave_data_visible;
                foreach (var m in map_wave_data_visible)
                {
                    if (ListView3biao(m.reward_name))
                    {
                        m.color = ColorTranslator.FromHtml("#d6d6d6");
                    }
                    m.SetSubItems();
                }
                listView3.VirtualListSize = map_wave_data_visible.Count;
                listView3.EndUpdate();
            }
            if (e.Button == MouseButtons.Right && listView2.SelectedIndices.Count > 0)
            {
                if (listView2.SelectedIndices.Count == 1)
                {
                    quest_data_List selectasset = (quest_data_List)listView2.Items[listView2.SelectedIndices[0]];
                    Clipboard.SetText(selectasset.quest_name + "");
                    MessageBox.Show("已复制名称" + "\n" + selectasset.count+ "\n" + selectasset.quest_idaddcount);
                }
            }
        }

        private void ListView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView3.BeginUpdate();
            listView3.SelectedIndices.Clear();

            if (listView2.SelectedIndices.Count > 0)
            {
                quest_data_List selectasset = (quest_data_List)listView2.Items[Convert.ToInt32(listView2.SelectedIndices[0])];

                map_wave_data_visible = selectasset.map_wave_data_visible;
                foreach (var m in map_wave_data_visible)
                {
                    if (ListView3biao(m.reward_name))
                    {
                        m.color = ColorTranslator.FromHtml("#d6d6d6");
                    }
                    //m.kucun = kucun_data_list.Find(x => x.field2.Equals(m.reward_name)).field3;
                    kucun_data_list.ForEach(x => {
                        //MessageBox.Show(x.field2);
                        if (x.field2.IndexOf(m.reward_name) != -1)
                        {
                            m.kucun = x.field3;
                        }
                    });

                    m.SetSubItems();
                }
            }

            listView3.VirtualListSize = map_wave_data_visible.Count;
            listView3.EndUpdate();
        }

        private void ListView4_MouseClick(object sender, MouseEventArgs e)
        {
            listView2.BeginUpdate();
            listView2.SelectedIndices.Clear();

            reward_id_list = new List<int>();
            resultList = new List<quest_data_List>();
            quest_data_visible = new List<quest_data_List>();

            if (e.Button == MouseButtons.Left | e.Button == MouseButtons.Right)
            {
                if (listView4.SelectedIndices.Count >= 1)
                {
                    var num = listView4.SelectedIndices.Count;

                    foreach (var v in listView4.SelectedIndices)
                    {
                        equipment_data_View selectasset = (equipment_data_View)listView4.Items[Convert.ToInt32(v.ToString())];
                        //MessageBox.Show(selectasset.equipment_id + "");
                        reward_id_list.Add(selectasset.equipment_id);
                    }
                    //MessageBox.Show(listView4.SelectedIndices.Count + "");
                    /*
                    reward_id_list.Add(115613);
                    reward_id_list.Add(125283);
                    */

                    var andList = new List<quest_data_List>();
                    var orList = new List<quest_data_List>();
                    var middleList = new List<quest_data_List>();

                    reward_id_list.ForEach(r =>
                    {
                        FilterQuest(r);
                    });
                    //MessageBox.Show(reward_id_list.Count + "");
                    if (quest_data_visible.Count > 0)
                    {
                        if (num == 1)
                        {
                            //MessageBox.Show(resultList.Count + "");
                            resultList = quest_data_visible;
                            foreach (var v in resultList)
                            {
                                v.color = ColorTranslator.FromHtml("#ffffff");
                                v.foreColor = ColorTranslator.FromHtml("#000000");
                                //MessageBox.Show(v.count + "");
                            }
                        }
                        else
                        {
                            resultList = new List<quest_data_List>();

                            quest_data_visible.ForEach(r =>
                            {
                                if (!andList.Contains(r) && !middleList.Contains(r))
                                {
                                    int f = Frequency(quest_data_visible, r);
                                    //MessageBox.Show(f + "");
                                    if (f == num)
                                    {
                                        andList.Add(r);
                                    }
                                    else if (f == 1)
                                    {
                                        orList.Add(r);
                                    }
                                    else
                                    {
                                        middleList.Add(r);
                                    }
                                }
                            });

                            if (andList.Count > 0)
                            {
                                foreach (var a in andList)
                                {
                                    a.color = ColorTranslator.FromHtml("#cc5f5f");
                                    a.foreColor = ColorTranslator.FromHtml("#ffffff");
                                    if (a.areaId > 11000 & a.areaId < 11999)
                                    {
                                        resultList.Add(a);
                                    }
                                }
                                //MessageBox.Show("全命中 andList:" + andList.Count);
                            }
                            if (middleList.Count > 0)
                            {
                                foreach (var m in middleList)
                                {
                                    m.color = ColorTranslator.FromHtml("#9f9f9f");
                                    m.foreColor = ColorTranslator.FromHtml("#ffffff");
                                    if (m.areaId > 11000 & m.areaId < 11999)
                                    {
                                        resultList.Add(m);
                                    }
                                }
                                //MessageBox.Show("多项命中 middleList:" + middleList.Count);

                            }
                            if (orList.Count > 0)
                            {
                                foreach (var o in orList)
                                {
                                    o.color = ColorTranslator.FromHtml("#ffffff");
                                    o.foreColor = ColorTranslator.FromHtml("#000000");
                                    if (o.areaId > 11000 & o.areaId < 11999)
                                    {
                                        resultList.Add(o);
                                    }
                                }
                                //MessageBox.Show("单项命中 orList:" + orList.Count);
                            }
                            //resultList = resultList.Distinct().ToList();
                            //MessageBox.Show("count");
                        }
                        foreach (var v in resultList)
                        {
                            List<Map_data> map_wave_data_visible_temp = new List<Map_data>();
                            v.wave_group_data_list.ForEach(w =>
                            {
                                w.enemy_reward_data_list.ForEach(en =>
                                {
                                    equipment_data_list.ForEach(eq =>
                                    {
                                        //MessageBox.Show(eq.equipment_id + "\n" + en.reward_id);
                                        if (eq.equipment_id % 10000 == en.reward_id % 10000)
                                        {
                                            map_wave_data_visible_temp.Add(new Map_data()
                                            {
                                                reward_name = eq.equipment_name,
                                                odds = en.odds
                                            });
                                        }
                                    });
                                });
                            });

                            int cou = 0;
                            foreach (var m in map_wave_data_visible_temp)
                            {
                                if (ListView3biao(m.reward_name))
                                {
                                    cou++;
                                }
                            }
                            v.count = cou;
                            v.quest_idaddcount= v.quest_id+cou*100000;
                            v.map_wave_data_visible = map_wave_data_visible_temp;
                        }

                        resultList.Sort((a, b) =>
                        {
                            var asf = a.quest_idaddcount;
                            var bsf = b.quest_idaddcount;
                            return reverseSort ? asf.CompareTo(bsf) : bsf.CompareTo(asf);
                        });

                        foreach (var v in resultList)
                        {
                            v.SetSubItems();
                        }
                    }
                }
            }
            if (e.Button == MouseButtons.Right && listView4.SelectedIndices.Count > 0)
            {
                //MessageBox.Show(equipment_data_visible.Count + "");
                if (listView4.SelectedIndices.Count == 1)
                {
                    equipment_data_View selectasset = (equipment_data_View)listView4.Items[listView4.SelectedIndices[0]];
                    Clipboard.SetText(selectasset.equipment_name);
                    MessageBox.Show("已复制名称");
                }
                else
                {
                    string cli = "";
                    foreach (var v in listView4.SelectedIndices)
                    {
                        equipment_data_View selectasset = (equipment_data_View)listView4.Items[Convert.ToInt32(v.ToString())];
                        cli += selectasset.equipment_name + ";";
                    }
                    Clipboard.SetText(cli);
                    MessageBox.Show("已复制多个名称");
                }
            }
            listView2.VirtualListSize = resultList.Count;
            listView2.EndUpdate();
        }

        private void ListView4_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView2.BeginUpdate();
            listView2.SelectedIndices.Clear();

            reward_id_list = new List<int>();
            resultList = new List<quest_data_List>();
            quest_data_visible = new List<quest_data_List>();

            if (listView4.SelectedIndices.Count >= 1)
            {
                var num = listView4.SelectedIndices.Count;

                foreach (var v in listView4.SelectedIndices)
                {
                    equipment_data_View selectasset = (equipment_data_View)listView4.Items[Convert.ToInt32(v.ToString())];
                    //MessageBox.Show(selectasset.equipment_id + "");
                    reward_id_list.Add(selectasset.equipment_id);
                }
                //MessageBox.Show(listView4.SelectedIndices.Count + "");
                /*
                reward_id_list.Add(115613);
                reward_id_list.Add(125283);
                */

                var andList = new List<quest_data_List>();
                var orList = new List<quest_data_List>();
                var middleList = new List<quest_data_List>();

                reward_id_list.ForEach(r =>
                {
                    FilterQuest(r);
                });
                //MessageBox.Show(reward_id_list.Count + "");
                if (quest_data_visible.Count > 0)
                {
                    if (num == 1)
                    {
                        //MessageBox.Show(resultList.Count + "");
                        resultList = quest_data_visible;
                        foreach (var v in resultList)
                        {
                            v.color = ColorTranslator.FromHtml("#ffffff");
                            v.foreColor = ColorTranslator.FromHtml("#000000");
                            //MessageBox.Show(v.count + "");
                        }
                    }
                    else
                    {
                        resultList = new List<quest_data_List>();

                        quest_data_visible.ForEach(r =>
                        {
                            if (!andList.Contains(r) && !middleList.Contains(r))
                            {
                                int f = Frequency(quest_data_visible, r);
                                //MessageBox.Show(f + "");
                                if (f == num)
                                {
                                    andList.Add(r);
                                }
                                else if (f == 1)
                                {
                                    orList.Add(r);
                                }
                                else
                                {
                                    middleList.Add(r);
                                }
                            }
                        });

                        if (andList.Count > 0)
                        {
                            foreach (var a in andList)
                            {
                                a.color = ColorTranslator.FromHtml("#cc5f5f");
                                a.foreColor = ColorTranslator.FromHtml("#ffffff");
                                resultList.Add(a);
                            }
                            //MessageBox.Show("全命中 andList:" + andList.Count);
                        }
                        if (middleList.Count > 0)
                        {
                            foreach (var m in middleList)
                            {
                                m.color = ColorTranslator.FromHtml("#9f9f9f");
                                m.foreColor = ColorTranslator.FromHtml("#ffffff");
                                resultList.Add(m);
                            }
                            //MessageBox.Show("多项命中 middleList:" + middleList.Count);

                        }
                        if (orList.Count > 0)
                        {
                            foreach (var o in orList)
                            {
                                o.color = ColorTranslator.FromHtml("#ffffff");
                                o.foreColor = ColorTranslator.FromHtml("#000000");
                                resultList.Add(o);
                            }
                            //MessageBox.Show("单项命中 orList:" + orList.Count);
                        }
                        //resultList = resultList.Distinct().ToList();
                        //MessageBox.Show("count");
                    }
                    foreach (var v in resultList)
                    {
                        List<Map_data> map_wave_data_visible_temp = new List<Map_data>();
                        v.wave_group_data_list.ForEach(w =>
                        {
                            w.enemy_reward_data_list.ForEach(en =>
                            {
                                equipment_data_list.ForEach(eq =>
                                {
                                    //MessageBox.Show(eq.equipment_id + "\n" + en.reward_id);
                                    if (eq.equipment_id % 10000 == en.reward_id % 10000)
                                    {
                                        map_wave_data_visible_temp.Add(new Map_data()
                                        {
                                            reward_name = eq.equipment_name,
                                            odds = en.odds
                                        });
                                    }
                                });
                            });
                        });

                        int cou = 0;
                        foreach (var m in map_wave_data_visible_temp)
                        {
                            if (ListView3biao(m.reward_name))
                            {
                                cou++;
                            }
                        }
                        v.count = cou;
                        v.map_wave_data_visible = map_wave_data_visible_temp;
                    }

                    resultList.Sort((a, b) =>
                    {
                        var asf = a.count;
                        var bsf = b.count;
                        return reverseSort ? asf.CompareTo(bsf) : bsf.CompareTo(asf);
                    });

                    foreach (var v in resultList)
                    {
                        v.SetSubItems();
                    }
                }

            }
            listView2.VirtualListSize = resultList.Count;
            listView2.EndUpdate();
        }

        private bool ListView3biao(string f)
        {
            bool ff = false;
            var ac = Regex.Replace(textBox2.Text, @"[0-9]", "");

            string[] Array = new string[] { };
            if (ac.IndexOf(";") != -1 | ac.IndexOf("；") != -1)
            {
                Array = ac.Split(';', '；');
            }
            else
            {
                if (f.IndexOf(textBox2.Text) != -1)
                {
                    ff = true;
                }
            }
            foreach (var a in Array)
            {
                if (!string.IsNullOrEmpty(a))
                {
                    if (f.IndexOf(a) != -1)
                    {
                        ff = true;
                    }
                    // MessageBox.Show(f + "\n" + a);
                }
            }

            if (listView4.SelectedIndices.Count >= 1)
            {
                foreach (var v in listView4.SelectedIndices)
                {
                    equipment_data_View selectasset = (equipment_data_View)listView4.Items[Convert.ToInt32(v.ToString())];
                    if (f.IndexOf(selectasset.equipment_name) != -1)
                    {
                        ff = true;
                    }
                }
            }
            return ff;
        }
        private readonly OpenFileDialog openFileDialogtest = new OpenFileDialog
        {
            AddExtension = false,
            Filter = "All types|*.*",
            Multiselect = true,
            RestoreDirectory = true
        };
        private void Button1_Click(object sender, EventArgs e)
        {
            openFileDialogtest.Filter = "(*.*)|*.*|(*.*)|*.*";
            if (openFileDialogtest.ShowDialog() == DialogResult.OK)
            {
                string strResult = openFileDialogtest.FileName;
                var str = File.ReadAllText(strResult);
                JObject jsonObj = JObject.Parse(str);
                var unit_list = jsonObj["data"]["unit_list"];
                var user_equip = jsonObj["data"]["user_equip"];
                JArray unit_lists = JArray.Parse(unit_list.ToString());
                JArray user_equips = JArray.Parse(user_equip.ToString());
                List<unit_list> unit_list_list = new List<unit_list>();
                List<user_equip> user_equip_list = new List<user_equip>();
                foreach (var unit in unit_lists)
                {
                    JArray equip_slots = JArray.Parse(unit["equip_slot"].ToString());

                    unit_list_list.Add(new unit_list()
                    {
                        id = Convert.ToInt32(unit["id"]),
                        rank = Convert.ToInt32(unit["promotion_level"]),
                        battle_rarity = Convert.ToInt32(unit["battle_rarity"]),
                        equip_slot0 = Convert.ToInt32(equip_slots[0]["is_slot"].ToString()),
                        equip_slot1 = Convert.ToInt32(equip_slots[1]["is_slot"].ToString()),
                        equip_slot2 = Convert.ToInt32(equip_slots[2]["is_slot"].ToString()),
                        equip_slot3 = Convert.ToInt32(equip_slots[3]["is_slot"].ToString()),
                        equip_slot4 = Convert.ToInt32(equip_slots[4]["is_slot"].ToString()),
                        equip_slot5 = Convert.ToInt32(equip_slots[5]["is_slot"].ToString())
                    });
                }
                foreach (var equip in user_equips)
                {
                    user_equip_list.Add(new user_equip()
                    {
                        id = Convert.ToInt32(equip["id"]),
                        stock = Convert.ToInt32(equip["stock"])
                    });
                }
                string DBfile = @"G:\原神\redive_cn.db";
                string unit_dataTableName = "unit_data_zh";
                string kucunTableName = "kucun";
                var connection = new SQLiteConnection(@"URI=file:" + DBfile);
                connection.Open();
                var command = connection.CreateCommand();

                var sqlComm = new SQLiteCommand("begin", connection);
                sqlComm.ExecuteNonQuery();

                foreach (var l in unit_list_list)
                {
                    try
                    {
                        command.CommandText = $"UPDATE {unit_dataTableName} SET rank = {l.rank} ,battle_rarityf = {l.battle_rarity} ,equip_slot1 = {l.equip_slot0} ,equip_slot2 = {l.equip_slot1} ,equip_slot3 = {l.equip_slot2} ,equip_slot4 = {l.equip_slot3}, equip_slot5 = {l.equip_slot4}, equip_slot6 = {l.equip_slot5}  WHERE unit_id = '{l.id}'";
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ee)
                    {
                        MessageBox.Show(ee.Message);
                    }
                }

                command.CommandText = "DELETE FROM kucun";
                command.ExecuteNonQuery();

                foreach (var u in user_equip_list)
                {
                    try
                    {
                        var eq = allequipment_data.Find(x => x.equipment_id.Equals(u.id));
                        if (u.id < 113011 & u.stock < 5)
                        {
                            continue;
                        }
                        command.CommandText = $"INSERT INTO {kucunTableName} VALUES ({eq.equipment_id},'{eq.equipment_name}',{u.stock})";
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ee)
                    {
                        MessageBox.Show(ee.Message);
                    }
                }
                sqlComm = new SQLiteCommand("end", connection);
                sqlComm.ExecuteNonQuery();
                connection.Close();
                REset();
                //MessageBox.Show("ok");
            }
           
        }


        static List<equipment_data_View> button2_data_list = new List<equipment_data_View>();

        private void Button2_Click(object sender, EventArgs e)
        {
            //textBox3.Text 数量
            //textBox4.Text 武器
            string wuqi = "";
            string shuliang = "";
            int shuliangs = 0;

            if (!string.IsNullOrEmpty(textBox4.Text))
            {
                wuqi = textBox4.Text;
            }
            if (!string.IsNullOrEmpty(textBox3.Text))
            {
                shuliang = textBox3.Text;
                shuliangs = Convert.ToInt32(shuliang);
            }

            string DBfile = @"G:\原神\redive_cn.db";
            string TableName = "kucun";

            if (!string.IsNullOrEmpty(wuqi))
            {
                if (!string.IsNullOrEmpty(shuliang))
                {
                    var ku = kucun_data_list.FindAll(x => x.field2.Equals(wuqi));
                    string UpdateDataSql;
                    string messageku;
                    if (ku.Count == 1)
                    {
                        UpdateDataSql = $"UPDATE {TableName} SET field3 = {shuliangs} WHERE field2 = '{ku[0].field2}'";
                        messageku = ku[0].field2;
                    }
                    else
                    {
                        var id = allequipment_data.Find(x => x.equipment_name.IndexOf(wuqi) != -1).equipment_id;
                        UpdateDataSql = $"INSERT INTO {TableName} VALUES ({id},'{wuqi}',{shuliangs})";
                        messageku = wuqi;
                    }
                    try
                    {
                        using (var connection = new SQLiteConnection(@"URI=file:" + DBfile))
                        {
                            connection.Open();

                            using (var command = connection.CreateCommand())
                            {
                                command.CommandText = UpdateDataSql;
                                var result = command.ExecuteNonQuery();

                                if (result != 1)
                                {
                                    MessageBox.Show("更新失败,可能武器名没对");
                                }
                                else if (result == 1)
                                {
                                    MessageBox.Show("更新完成\n" + messageku + "\n更新为" + shuliangs);
                                }
                            }

                            connection.Close();
                        }
                    }
                    catch
                    {
                        MessageBox.Show("未知错误");
                    }
                }
                else
                {
                    MessageBox.Show("输入更新什么武器的碎片量");
                }
            }
            else
            {
                MessageBox.Show("?? 输入什么武器啊");
            }

            /*var s = Interaction.MsgBox("是:更新数据库武器碎片数量" + "\n" + "否:制作了\"什么\"武器", MsgBoxStyle.YesNoCancel);
            if (s == MsgBoxResult.Yes)
            {
                
            }
            else if (s == MsgBoxResult.No)
            {
                
            }
            else if (c == MsgBoxResult.Cancel)
            { 

            }*/
        }

        public void Button2Addequipment_data(int equipment_id)
        {
            var v = equipment_craft_list.Find(
            x => (x.equipment_id % 10000).Equals(equipment_id % 10000) );

            if (v != null)
            {
                var ind = Indexfor(equipment_id);
                if (ind != null)
                {
                    button2_data_list.Add(ind);
                }

                if (v.condition_equipment_id_2 != 0)
                {
                    Button2Addequipment_data(v.condition_equipment_id_2);
                }
                if (v.condition_equipment_id_3 != 0)
                {
                    Button2Addequipment_data(v.condition_equipment_id_3);
                }
                if (v.condition_equipment_id_4 != 0)
                {
                    Button2Addequipment_data(v.condition_equipment_id_4);
                }
            }
            else
            {
                button2_data_list.Add(new equipment_data_View()
                {
                    equipment_id = equipment_id,

                    equipment_name = equipment_data_list.Find(
                        x => x.equipment_id == equipment_id).equipment_name,

                    consume_num_1 = 1
                });
            }
        }

        void Button2dell(string wuqi, string TableName, string DBfile, bool rank, equipment_data_View edv)
        {
            var f = equipment_data_list.Find(x => x.equipment_name.IndexOf(wuqi) != -1);
            if (f != null)
            {
                var connection = new SQLiteConnection(@"URI=file:" + DBfile);
                connection.Open();
                var command = connection.CreateCommand();
                button2_data_list = new List<equipment_data_View>();

                Button2Addequipment_data(f.equipment_id);
                //MessageBox.Show(button2_data_list.Count+"");
                var sqlComm = new SQLiteCommand("begin", connection);
                sqlComm.ExecuteNonQuery();

                foreach (var eq in button2_data_list)
                {
                    var c = kucun_data_list.Find(x => eq.equipment_id % 10000 == x.field1 % 10000);
                    if (c != null)
                    {
                        var equ = equipment_data_list.Find(x => x.equipment_id % 10000 == eq.equipment_id % 10000);
                        if (rank)
                        {
                            c.field3 -= edv.consume_num_1;
                            //MessageBox.Show("武器:" + c.field2 + "\n减去:" + edv.consume_num_1 + "\n减去后:" + c.field3);
                        }
                        else
                        {
                            c.field3 -= eq.consume_num_1;
                            //MessageBox.Show("武器:" + c.field2 + "\n减去:" + eq.consume_num_1 + "\n减去后:" + c.field3);
                        }
                        string UpdateDataSql = $"UPDATE {TableName} SET field3 = {c.field3} WHERE field2 = '{c.field2}'";

                        command.CommandText = UpdateDataSql;
                        int result = command.ExecuteNonQuery();
                        if (result != 1)
                        {
                            MessageBox.Show("更新失败,可能武器名没对\n" + c.field3);
                        }
                        else if (result == 1)
                        {
                            //MessageBox.Show("更新完成");
                        }
                    }
                    else
                    {

                        MessageBox.Show("找不到这个武器" + eq.equipment_id);
                    }
                }
                //connection.Close();
                sqlComm = new SQLiteCommand("end", connection);
                sqlComm.ExecuteNonQuery();
            }
            else
            {
                MessageBox.Show("找不到这个武器");
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            REset();
        }

        void REset()
        {
            equipmentFiltering = false;
            textBox2.Text = "输入武器查询(多个请用;分开)";
            textBox2.ForeColor = SystemColors.GrayText;

            enableFiltering = false;
            textBox1.Text = "输入角色查询(多个请用;分开)";
            textBox1.ForeColor = SystemColors.GrayText;

            textBox3.Text = "";
            textBox4.Text = "";

            equipmentFilteringFiltering = false;
            textBox5.Text = "武器查询的查询";
            textBox5.ForeColor = SystemColors.GrayText;

            questFiltering = false;
            textBox6.Text = "关卡查询";
            textBox6.ForeColor = SystemColors.GrayText;

            BuildAssetStructures();
        }

        private void TextBox4_Enter(object sender, EventArgs e)
        {
            textBox4.Text = "";
            textBox3.Text = "";
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            listView4.BeginUpdate();
            listView4.SelectedIndices.Clear();
            textBox2.Text = "输入武器查询(多个请用;分开)";
            textBox2.ForeColor = SystemColors.GrayText;
            equipment_data_visible = new List<equipment_data_View>();

            List<int> dontup = new List<int>()
            {
                100401,//未奏希
                100801,//雪
                105001,//美咲
                106401,//雪菲
                106701,//帆稀
                107601,//可可萝（夏日）
                107701,//铃莓（夏日）
                107701,//铃莓
                108101,//忍（万圣节）
                108301,//美咲（万圣节）
                108501,//胡桃（圣诞节）
                109001,//惠理子（情人节）
                109301,//露
                109801,//拉姆
                109901,//爱蜜莉雅
                110001,//铃奈（夏日）
                110201,//美咲（夏日）
                110901,//千爱瑠
                111601,//望（圣诞节）
                112101,//铃莓（新年）
                112301,//栞（魔法少女）
                112401,//卯月（偶像大师）
                112501,//凛（偶像大师）
                112601,//未央（偶像大师）
                112701,//铃（游侠）
                112901,//璃乃（奇幻）
                113001,//步美（奇幻）
                106601,//祈梨
                113201,//杏奈（夏日）
                113301,//七七香（夏日）
                113501,//美里（夏日）
                113601,//纯（夏日）
                113701,//茜里（天使）
                114001,//茉莉（万圣节）
                114101,//怜（万圣节）
                114301,//智（魔法少女）
                114401,//秋乃（圣诞节）
                114601,//由加莉（圣诞节）

                180501,//可可萝（公主）
                110301,//咲恋（夏日）
                108401,//千歌（圣诞节）
                100201,//优衣
                101001,//真步
                104201,//千歌
                109101,//静流（情人节）
                100601,//茜里
                101401,//香澄
                105901,//可可萝
                107001,//似似花
                111401,//露娜
                111101,//镜华（万圣节）
                108801,//优衣（新年）
                102601,//铃
            };

            foreach (var ae in unit_datavisible)
            {
                if (dontup.Exists(x => x == ae.unit_id))
                {
                    continue;
                }
                if (ae.rank >= 4 && ae.rank <= rank)
                {
                    equipment_data_visible = Listview1(ae, ae.rank, rank);
                }
            }

            equipment_data_visible.Sort((a, b) =>
            {
                var asf = a.jixu;
                var bsf = b.jixu;
                //return reverseSort ? asf.CompareTo(bsf) : bsf.CompareTo(asf);
                return reverseSort ? bsf.CompareTo(asf) : asf.CompareTo(bsf);
            });


            int vis = 0;
            int zongshu = 0;
            var temp = new List<equipment_data_View>();
            //var tes = "";
            foreach (var v in equipment_data_visible)
            {
                if (v.haixu > 0)
                {
                    temp.Add(v);
                    vis++;
                    zongshu += v.haixu;
                }
            }
            equipment_data_visible = temp;

            foreach (var v in equipment_data_visible)
            {
                v.SetSubItems();
            }

            label6.Text = $"缺{vis}个装备\n合计{zongshu}碎片\n";

            listView4.VirtualListSize = vis;
            listView4.EndUpdate();
        }

        private void ListView3_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && listView3.SelectedIndices.Count > 0)
            {
                //MessageBox.Show(equipment_data_visible.Count + "");
                if (listView3.SelectedIndices.Count == 1)
                {
                    Map_data selectasset = (Map_data)listView3.Items[listView3.SelectedIndices[0]];
                    Clipboard.SetText(selectasset.reward_name);
                    MessageBox.Show("已复制名称");
                }
            }
        }

        private void TextBox5_Enter(object sender, EventArgs e)
        {
            if (textBox5.Text == "武器查询的查询")
            {
                textBox5.Text = "";
                textBox5.ForeColor = SystemColors.WindowText;
                equipmentFilteringFiltering = true;
            }
        }

        private void TextBox5_Leave(object sender, EventArgs e)
        {
            if (textBox5.Text == "")
            {
                equipmentFilteringFiltering = false;
                textBox5.Text = "武器查询的查询";
                textBox5.ForeColor = SystemColors.GrayText;
            }
        }

        private void TextBox5_TextChanged(object sender, EventArgs e)
        {
            if (equipmentFilteringFiltering)
            {
                if (TimerEquipmentFiltering.Enabled)
                {
                    TimerEquipmentFiltering.Stop();
                    TimerEquipmentFiltering.Start();
                }
                else
                {
                    TimerEquipmentFiltering.Start();//fin
                }
            }
        }

        private void TimerEquipmentFiltering_Elapsed(object sender, ElapsedEventArgs e)
        {
            TimerEquipmentFiltering.Stop();
            Invoke(new Action(FilterFilterEquipment));
        }

        void FilterFilterEquipment()
        {
            if (textBox5.Text != "武器查询的查询" && !string.IsNullOrEmpty(textBox5.Text))
            {
                string s = "";
                for (int i = 0; i < listView4.Items.Count; i++)
                {
                    var v = (equipment_data_View)listView4.Items[i];
                    if (v.equipment_name.IndexOf(textBox5.Text) != -1)
                    {
                        s += v.equipment_name + " " + v.consume_num_1 + "\n";
                    }
                }

                if (!string.IsNullOrEmpty(s))
                {
                    MessageBox.Show(s);
                }
                else
                {
                    MessageBox.Show("不需要或者没有");
                }
            }
        }


        private void TextBox6_TextChanged(object sender, EventArgs e)
        {
            if (questFiltering)
            {
                if (TimerquestFiltering.Enabled)
                {
                    TimerquestFiltering.Stop();
                    TimerquestFiltering.Start();
                }
                else
                {
                    TimerquestFiltering.Start();//fin
                }
            }
        }

        private void TextBox6_Leave(object sender, EventArgs e)
        {
            //if (textBox6.Text == "")
            //{
            questFiltering = false;
            textBox6.Text = "关卡查询";
            textBox6.ForeColor = SystemColors.GrayText;
            //}
        }

        private void TextBox6_Enter(object sender, EventArgs e)
        {
            if (textBox6.Text == "关卡查询")
            {
                textBox6.Text = "";
                textBox6.ForeColor = SystemColors.WindowText;
                questFiltering = true;
            }
        }

        private void TimerquestFiltering_Elapsed(object sender, ElapsedEventArgs e)
        {
            TimerquestFiltering.Stop();
            Invoke(new Action(QuestFilterings));
        }
        void QuestFilterings()
        {
            if (textBox6.Text != "关卡查询" && !string.IsNullOrEmpty(textBox6.Text))
            {
                int s = 0;
                string name = "";
                quest_data_List f = new quest_data_List();
                for (int i = 0; i < listView2.Items.Count; i++)
                {

                    var v = (quest_data_List)listView2.Items[i];
                    if (v.quest_id > 11100000)
                    {
                        continue;
                    }
                    if (textBox6.Text.IndexOf("-") != -1)
                    {
                        if (v.quest_name.EndsWith(textBox6.Text))
                        {
                            f = v;
                            name += v.Text + " " + v.quest_name + "\n";
                            s++;
                        }
                        //MessageBox.Show(v.quest_name.LastIndexOf(textBox6.Text)+"");
                    }
                    else
                    {
                        if (v.quest_name.IndexOf(textBox6.Text) != -1)
                        {
                            f = v;
                            name += v.Text + " " + v.quest_name + "\n";
                            s++;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(name))
                {
                    MessageBox.Show(name);
                }
                else
                {
                    MessageBox.Show("没有");
                }
                if (s == 1)
                {
                    listView3.BeginUpdate();
                    listView3.SelectedIndices.Clear();

                    map_wave_data_visible = f.map_wave_data_visible;
                    foreach (var m in map_wave_data_visible)
                    {
                        if (ListView3biao(m.reward_name))
                        {
                            m.color = ColorTranslator.FromHtml("#d6d6d6");
                        }
                        kucun_data_list.ForEach(x => {
                            if (x.field2.IndexOf(m.reward_name) != -1)
                            {
                                m.kucun = x.field3;
                            }
                        });
                        m.SetSubItems();
                    }
                    listView3.VirtualListSize = map_wave_data_visible.Count;
                    listView3.EndUpdate();
                }
            }
        }

        private void 计算体力_Click(object sender, EventArgs e)
        {
            /*DateTime 左一 = new DateTime(2022, 3, 15, 15, 09, 00);//54
            DateTime 左二 = new DateTime(2022, 3, 15, 15, 59, 00);//12
            DateTime 左三 = new DateTime(2022, 3, 15, 14, 15, 00);
            DateTime 左四 = new DateTime(2022, 3, 15, 15, 59, 00);
            DateTime 左五 = new DateTime(2022, 3, 15, 15, 59, 00);
            DateTime 左六 = new DateTime(2022, 3, 15, 16, 14, 00);
            DateTime 左七 = new DateTime(2022, 3, 15, 17, 53, 00);

            int 左一t = 54;
            int 左二t = 12;
            int 左三t = 12;
            int 左四t = 12;
            int 左五t = 12;
            int 左六t = 12;
            int 左七t = 12;

            while (左一t < 216)
            {
                左一 = 左一.AddHours(6);
                左一t += 54;
            }
            while (左二t < 48)
            {
                左二 = 左二.AddHours(6);
                左二t += 12;
            }
            while (左三t < 48)
            {
                左三 = 左三.AddHours(6);
                左三t += 12;
            }
            while (左四t < 48)
            {
                左四 = 左四.AddHours(6);
                左四t += 12;
            }
            while (左五t < 48)
            {
                左五 = 左五.AddHours(6);
                左五t += 12;
            }
            while (左六t < 48)
            {
                左六 = 左六.AddHours(6);
                左六t += 12;
            }
            while (左七t < 48)
            {
                左七 = 左七.AddHours(6);
                左七t += 12;
            }


            MessageBox.Show(左一 + "\n" +
               左二 + "\n" +
               左三 + "\n" +
               左四 + "\n" +
               左五 + "\n" +
               左六 + "\n" +
               左七 + "\n");*/
            try
            {
                var ti = Convert.ToInt32(Interaction.InputBox("请输入现在体力"));
                
                int ho = (int)numericUpDown1.Value;
                DateTime timer = DateTime.Now;
                DateTime jiudian;
                if (timer.Hour> ho)
                {
                    jiudian = new DateTime(timer.Year, timer.Month, timer.Day , ho, 0, 0).AddDays(1);
                }
                else
                {
                    jiudian = new DateTime(timer.Year, timer.Month, timer.Day, ho, 0, 0);
                }
                TimeSpan cha = jiudian - timer;
                var time = cha.Minutes + 60 * cha.Hours;//还差这么多分钟到早上9点

                var chatili = zongtili - ti;
                var chatime = chatili * 6;//还差这么多分钟体力就满
                var newtili = time / 6 + ti;//届时大概体力
                ///还差30分钟到9点但是还有6分钟体力就满了
                ///30-6=24分钟 放心不会超

                ///还差6分钟到9点但是还有30分钟体力就满了
                ///6-30= -24

                if (time - chatime > 0)
                {
                    MessageBox.Show($"还有{chatime}分钟体力就满了\n届时体力{newtili}\n削减体力到{zongtili - time / 6 - 1}吧");
                }
                else
                {
                    MessageBox.Show($"还差{time}分钟到{ho}点\n届时体力{newtili}\n没问题");
                }

                //MessageBox.Show(time.ToString());
            }
            catch (Exception ee){
                MessageBox.Show(ee.Message);
            }
            
        }
        class unit_list
        {
            public int id { get; set; }
            public int rank { get; set; }
            public int battle_rarity { get; set; }
            public int equip_slot0 { get; set; }

            public int equip_slot1 { get; set; }
            public int equip_slot2 { get; set; }
            public int equip_slot3 { get; set; }
            public int equip_slot4 { get; set; }
            public int equip_slot5 { get; set; }
        }

        class user_equip
        {
            public int id;
            public int stock;
        }
        

        private void button7_Click(object sender, EventArgs e)
        {
            
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && listView1.SelectedIndices.Count > 0)
            {
                //MessageBox.Show(equipment_data_visible.Count + "");
                if (listView1.SelectedIndices.Count == 1)
                {
                    unit_data selectasset = (unit_data)listView1.Items[listView1.SelectedIndices[0]];
                    //Clipboard.SetText(selectasset.unit_name);
                    //MessageBox.Show("已复制名称");
                    MessageBox.Show(selectasset.equip_slot1+"");
                }
            }
        }
    }
}