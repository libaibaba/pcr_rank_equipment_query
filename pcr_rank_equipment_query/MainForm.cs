using System;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;
using System.IO;
using static pcr_rank_equipment_query.Load;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace pcr_rank_equipment_query
{
    public partial class MainForm : Form
    {
        private System.Timers.Timer delayTimer = new System.Timers.Timer(800) { Enabled = true };
        private System.Timers.Timer TimerEquipment = new System.Timers.Timer(800) { Enabled = true };
        public bool enableFiltering = false;
        public bool equipmentFiltering = false;
        List<equipment_data_View> equipment_data_visible = new List<equipment_data_View>();

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
            int comboBox1i = 2;
            while (comboBox1.Items.Count < 15)
            {
                comboBox1.Items.Add(comboBox1i);
                comboBox1i++;
            }
            int comboBox2i = 14;
            while (comboBox2.Items.Count < 15)
            {
                comboBox2.Items.Add(comboBox2i);
                comboBox2i--;
            }
            BuildAssetStructures();
            
        }
        private async void BuildAssetStructures()
        {
            foreach (ColumnHeader ch in listView1.Columns)
            {
                ch.Width = listView1.Width / listView1.Columns.Count;
            }
            foreach (ColumnHeader ch in listView4.Columns)
            {
                ch.Width = listView4.Width / listView4.Columns.Count;
            }
            /*foreach (ColumnHeader ch in listView2.Columns)
            {
                ch.Width = listView2.Width / listView2.Columns.Count;
            }*/
            listView2.Columns[0].Width = 213;
            listView2.Columns[1].Width = 63;

            await Task.Run(() => SaveDB());
            listView1.VirtualListSize = unit_datavisible.Count;
            LoadAlleq();
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            delayTimer.Elapsed += new ElapsedEventHandler(delayTimer_Elapsed);
            TimerEquipment.Elapsed += new ElapsedEventHandler(TimerEquipment_Elapsed);
        }
        private void delayTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            delayTimer.Stop();
            Invoke(new Action(FilterCharacter));
        }
        private void TimerEquipment_Elapsed(object sender, ElapsedEventArgs e)
        {
            TimerEquipment.Stop();
            Invoke(new Action(FilterEquipment));
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                enableFiltering = false;
                textBox1.Text = "输入角色查询";
                textBox1.ForeColor = SystemColors.GrayText;
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "输入角色查询")
            {
                textBox1.Text = "";
                textBox1.ForeColor = SystemColors.WindowText;
                enableFiltering = true;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
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

        void FilterCharacter()
        {
            listView1.BeginUpdate();
            listView1.SelectedIndices.Clear();
            unit_datavisible = unit_data_list;
            if (textBox1.Text != "输入角色查询")
            {
                unit_datavisible = unit_datavisible.FindAll(
                        x => x.SubItems[0].Text.IndexOf(textBox1.Text, StringComparison.OrdinalIgnoreCase) >= 0|
                        x.SubItems[1].Text.IndexOf(textBox1.Text) >= 0);
            }
            listView1.VirtualListSize = unit_datavisible.Count;
            listView1.EndUpdate();
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

                equipment_data_visible = new List<equipment_data_View>();
                string[] Array;
                if (textBox2.Text.IndexOf(";")!=-1)
                {
                    Array = textBox2.Text.Split(';');
                    foreach (var a in Array)
                        if (!string.IsNullOrEmpty(a))
                        {
                            listView4Array(a);
                            //MessageBox.Show(a);
                        }
                }
                else if (textBox2.Text.IndexOf("；") != -1)
                {
                    Array = textBox2.Text.Split('；');
                    foreach (var a in Array)
                        if (!string.IsNullOrEmpty(a))
                        {
                            listView4Array(a);
                            //MessageBox.Show(a);
                        }
                }
                else
                {
                    if (!string.IsNullOrEmpty(textBox2.Text))
                    {
                        listView4Array(textBox2.Text);
                    }
                }
                foreach (var a in equipment_data_visible)
                {
                    a.SetSubItems();
                }
            }
            listView4.VirtualListSize = equipment_data_visible.Count;
            listView4.EndUpdate();
        }

        void listView4Array(string f)
        {
            var FindAllequipment = equipment_data_list.FindAll(x => x.equipment_name.IndexOf(f) >= 0);
            foreach (var adde in FindAllequipment)
            {
                equipment_data_visible.Add(new equipment_data_View()
                {
                    equipment_id = adde.equipment_id,
                    equipment_name = adde.equipment_name,
                    consume_num_1 = 1
                });
            }
        }

        private void listView1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            //MessageBox.Show("d"+ e.ItemIndex);
            e.Item = unit_datavisible[e.ItemIndex];
        }

        private void listView4_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = equipment_data_visible[e.ItemIndex];
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            listView4.BeginUpdate();
            listView4.SelectedIndices.Clear();

            equipment_data_visible = new List<equipment_data_View>();

            var filterunit_promotion = unit_promotion_list;
            filterunit_promotion = filterunit_promotion.FindAll(
                        x => x.unit_id == ((unit_data)e.Item).unit_id &
                        x.promotion_level >= int.Parse(comboBox1.Text) &
                        x.promotion_level <= int.Parse(comboBox2.Text));

            for (int i = 0; i < filterunit_promotion.Count; i++)
            {
                Addequipment_data_View(filterunit_promotion[i].equip_slot_1);
                Addequipment_data_View(filterunit_promotion[i].equip_slot_2);
                Addequipment_data_View(filterunit_promotion[i].equip_slot_3);
                Addequipment_data_View(filterunit_promotion[i].equip_slot_4);
                Addequipment_data_View(filterunit_promotion[i].equip_slot_5);
                Addequipment_data_View(filterunit_promotion[i].equip_slot_6);
            }

            foreach (var v in equipment_data_visible)
            {
                v.SetSubItems();
            }

            listView4.VirtualListSize = equipment_data_visible.Count;
            listView4.EndUpdate();
        }

        public void Addequipment_data_View(int equipment_id)
        {
            var v = equipment_craft_list.Find(
            x => x.equipment_id == equipment_id);

            if (v != null)
            {
                equipment_data_visible.Add(Indexfor(equipment_id));
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
            equipment_data_View v = new equipment_data_View()
            {
                equipment_id = equipment_id,

                equipment_name = equipment_data_list.Find(
            x => x.equipment_id == equipment_craft_list.Find(c => c.equipment_id == equipment_id).condition_equipment_id_1).equipment_name,

                consume_num_1 = equipment_craft_list.Find(
            x => x.equipment_id == equipment_id).consume_num_1

            };
            return v;
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
                            /*if (en.odds != 72)
                            {
                                MessageBox.Show(q.quest_id+"\n"+
                                   w.wave_group_id +"\n"
                                    +en.drop_reward_id+"\n"+
                                    en.reward_id % 10000 + "\n" +
                                    en.reward_id + "\n" +
                                    reward_id.equipment_id % 10000 + "\n" +
                                    reward_id.equipment_id + "\n");
                            }*/
                            #endregion
                            q.odds = en.odds;
                            quest_data_visible.Add(q);
                        }
                    });
                });
            });
        }

        private void listView2_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = resultList[e.ItemIndex];
        }

        private void listView2_ColumnClick(object sender, ColumnClickEventArgs e)
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
            if (sortColumn == 1)
            {
                //MessageBox.Show("?");
                resultList.Sort((a, b) =>
                {
                    var asf = a.odds;
                    var bsf = b.odds;
                    return reverseSort ? bsf.CompareTo(asf) : asf.CompareTo(bsf);
                });
            }
            listView2.EndUpdate();
        }

        private void listView4_MouseClick(object sender, MouseEventArgs e)
        {
            reward_id_list = new List<int>();
            resultList = new List<quest_data_List>();
            quest_data_visible = new List<quest_data_List>();


            if (e.Button == MouseButtons.Left && listView4.SelectedIndices.Count > 0)
            {
                if (listView4.SelectedIndices.Count >= 1)
                {
                    foreach (var v in listView4.SelectedIndices)
                    {
                        equipment_data_View selectasset = (equipment_data_View)listView4.Items[Convert.ToInt32(v.ToString())];
                        //MessageBox.Show(selectasset.equipment_id + "");
                        reward_id_list.Add(selectasset.equipment_id);
                    }

                    /*
                    reward_id_list.Add(115613);
                    reward_id_list.Add(125283);
                    */

                    listView2.BeginUpdate();
                    listView2.SelectedIndices.Clear();

                    var andList = new List<quest_data_List>();
                    var orList = new List<quest_data_List>();
                    var middleList = new List<quest_data_List>();

                    reward_id_list.ForEach(r =>
                    {
                        FilterQuest(r);
                    });

                    if (quest_data_visible.Count > 0)
                    {
                        var num = quest_data_visible.Count;
                        if (num == 1)
                        {
                            resultList = quest_data_visible;
                            foreach (var v in resultList)
                            {
                                v.SetSubItems();
                            }
                        }
                        else
                        {
                            quest_data_visible.ForEach(r => {
                                if (!andList.Contains(r) && !middleList.Contains(r))
                                {
                                    int f = frequency(quest_data_visible, r);
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
                                    a.color = ColorTranslator.FromHtml("#9f9f9f");
                                    resultList.Add(a);
                                }
                                //MessageBox.Show("全命中 andList:" + andList.Count);
                            }
                            if (middleList.Count > 0)
                            {
                                foreach (var m in middleList)
                                {
                                    m.color = ColorTranslator.FromHtml("#d6d6d6");
                                    resultList.Add(m);
                                }
                                //MessageBox.Show("多项命中 middleList:" + middleList.Count);
                            }
                            if (orList.Count > 0)
                            {
                                foreach (var o in orList)
                                {
                                    o.color = ColorTranslator.FromHtml("#ffffff");
                                    resultList.Add(o);
                                }
                                //MessageBox.Show("单项命中 orList:" + orList.Count);
                            }
                            resultList.Sort((a, b) =>
                            {
                                var asf = a.odds;
                                var bsf = b.odds;
                                return reverseSort ? asf.CompareTo(bsf) : bsf.CompareTo(asf);
                            });
                            resultList = resultList.Distinct().ToList();
                            foreach (var v in resultList)
                            {
                                v.SetSubItems();
                            }
                        }
                    }
                    listView2.VirtualListSize = resultList.Count;
                    listView2.EndUpdate();
                }
            }
            if (e.Button == MouseButtons.Right && listView4.SelectedIndices.Count > 0)
            {
                if (listView4.SelectedIndices.Count == 1) {
                    equipment_data_View selectasset = (equipment_data_View)listView4.Items[listView4.SelectedIndices[0]];
                    Clipboard.SetText(selectasset.equipment_name);
                    MessageBox.Show("已复制名称");
                }
            }
        }

        int frequency(List<quest_data_List> c, quest_data_List o)
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

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "输入武器查询(多个请用;分开)")
            {
                textBox2.Text = "";
                textBox2.ForeColor = SystemColors.WindowText;
                equipmentFiltering = true;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
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

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                equipmentFiltering = false;
                textBox2.Text = "输入武器查询(多个请用;分开)";
                textBox2.ForeColor = SystemColors.GrayText;

                listView1.BeginUpdate();
                listView1.SelectedIndices.Clear();
                listView1.EndUpdate();

                LoadAlleq();
            }
        }

        void LoadAlleq()
        {

            listView4.BeginUpdate();
            listView4.SelectedIndices.Clear();

            equipment_data_visible = new List<equipment_data_View>();
            foreach (var eq in equipment_data_list)
            {
                equipment_data_visible.Add(new equipment_data_View()
                {
                    equipment_id = eq.equipment_id,
                    equipment_name = equipment_data_list.Find(
                        x => x.equipment_id == eq.equipment_id).equipment_name,
                    consume_num_1 = 1
                });
            }

            foreach (var a in equipment_data_visible)
            {
                a.SetSubItems();
            }
            listView4.VirtualListSize = equipment_data_visible.Count;
            listView4.EndUpdate();
        }
    }
}
