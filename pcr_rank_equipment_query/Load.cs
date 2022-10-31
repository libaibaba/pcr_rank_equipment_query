using Dapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace pcr_rank_equipment_query
{
    internal static class Load
    {
        //static string DBfile = Path.Combine(Path.GetTempPath(), "redive_cn.db");
        static readonly string DBfile = @"G:\原神\redive_cn.db";
        public static ArrayList temp = new ArrayList();
        public static List<equipment_data> equipment_data_list = new List<equipment_data>();//装备名字清单
        public static List<unit_data> unit_data_list = new List<unit_data>();//人物清单
        public static List<unit_promotion> unit_promotion_list = new List<unit_promotion>();//人物rank所需清单
        public static List<equipment_craft> equipment_craft_list = new List<equipment_craft>();//制作装备所需清单
        //public static List<quest_data> quest_data_list = new List<quest_data>();//关卡掉落清单
        //public static List<enemy_reward_data> enemy_reward_data_list = new List<enemy_reward_data>();//掉落物品及概率
        //public static List<wave_group_data> wave_group_data_list = new List<wave_group_data>();//总掉落清单
        public static List<kucun_data> kucun_data_list = new List<kucun_data>();

        public static int 地图 = 36;
        public static int rank = 17;
        public static int zongtili = 224;

        public static List<unit_data> unit_datavisible = new List<unit_data>();
        public static List<quest_data_List> quest_data_lists = new List<quest_data_List>();
        public static List<enemy_reward_data_List> enemy_reward_data_lists = new List<enemy_reward_data_List>();
        public static List<wave_group_data_List> wave_group_data_lists = new List<wave_group_data_List>();
        public static List<equipment_data> allequipment_data = new List<equipment_data>();
        public static Dictionary<int, int> temp2 = new Dictionary<int, int>();


        static readonly string[] loadfilestring = new string[]{ "地图", "36", "rank", "17", "总体力", "224" };
        static readonly string loadfilepath = "load.txt";

        public static string SaveDB()
        {
            if (!File.Exists(DBfile))
            {
                File.WriteAllBytes(DBfile, Properties.Resources.redive_cn);
            }
            LoadTxt();
            LoadDB();
            return "ok";
        }

        public static void LoadTxt()
        {
            if (!File.Exists(loadfilepath))
            {
                File.WriteAllLines(loadfilepath, loadfilestring);
            }

            string[] loadfilestringnew = File.ReadAllLines(loadfilepath);
            地图 = Convert.ToInt32(loadfilestringnew[1]);
            rank = Convert.ToInt32(loadfilestringnew[3]);
            zongtili = Convert.ToInt32(loadfilestringnew[5]);
        }

        public static void enemy_rewardvoid(int key,int value)
        {
            if ((value % 100000 / 1000) > 地图)//地图限制
            {
                return;
            }

            if (!key.Equals(0)& value<11999091)
            {
                if (temp2.ContainsKey(key))
                {
                    if (temp2[key] < value)
                    {
                        temp2[key] = value;
                    }
                }
                else
                {
                    temp2.Add(key, value);
                }
            }
        }
        public static void LoadDB()
        {
            temp = new ArrayList();
            equipment_data_list = new List<equipment_data>();
            unit_data_list = new List<unit_data>();
            unit_promotion_list = new List<unit_promotion>();
            equipment_craft_list = new List<equipment_craft>();
            //quest_data_list = new List<quest_data>();
            //enemy_reward_data_list = new List<enemy_reward_data>();
            //wave_group_data_list = new List<wave_group_data>();
            kucun_data_list = new List<kucun_data>();
            unit_datavisible = new List<unit_data>();
            quest_data_lists = new List<quest_data_List>();
            enemy_reward_data_lists = new List<enemy_reward_data_List>();
            wave_group_data_lists = new List<wave_group_data_List>();
            allequipment_data = new List<equipment_data>();


            var DBFile = new SQLiteConnection(@"URI=file:" + DBfile);
            var equipment_data = DBFile.Query<equipment_data>("SELECT equipment_id,equipment_name FROM equipment_data", new DynamicParameters());
            var unit_data = DBFile.Query<unit_data>("SELECT unit_id,unit_name,bieming,rank,battle_rarityf,equip_slot1,equip_slot2,equip_slot3,equip_slot4,equip_slot5,equip_slot6 FROM unit_data_zh", new DynamicParameters());
            var unit_promotion = DBFile.Query<unit_promotion>("SELECT unit_id,promotion_level,equip_slot_1,equip_slot_2,equip_slot_3,equip_slot_4,equip_slot_5,equip_slot_6 FROM unit_promotion", new DynamicParameters());
            var equipment_craft = DBFile.Query<equipment_craft>("SELECT equipment_id,condition_equipment_id_1,consume_num_1,condition_equipment_id_2,consume_num_2,condition_equipment_id_3,consume_num_3,condition_equipment_id_4,consume_num_4 FROM equipment_craft", new DynamicParameters());
            var quest_data = DBFile.Query<quest_data>("SELECT quest_id,area_id,quest_name,wave_group_id_1,wave_group_id_2,wave_group_id_3,reward_image_1,reward_image_2,reward_image_3,reward_image_4,reward_image_5 FROM quest_data", new DynamicParameters());
            var enemy_reward_data = DBFile.Query<enemy_reward_data>("SELECT drop_reward_id,reward_id_1,odds_1,reward_id_2,odds_2,reward_id_3,odds_3,reward_id_4,odds_4,reward_id_5,odds_5 FROM enemy_reward_data", new DynamicParameters());
            var wave_group_data = DBFile.Query<wave_group_data>("SELECT wave_group_id,drop_reward_id_1,drop_reward_id_2,drop_reward_id_3,drop_reward_id_4,drop_reward_id_5 FROM wave_group_data", new DynamicParameters());
            var kucun_data = DBFile.Query<kucun_data>("SELECT field1,field2,field3 FROM kucun", new DynamicParameters());
            DBFile.Close();

            kucun_data_list = kucun_data.ToList();

            allequipment_data = equipment_data.ToList();

            /*List<int> bucunz = new List<int>() {
                   103613,//只能合成
            };
            

            foreach (var q in equipment_data.ToList())
            {
                if (bucunz.Exists(x => x == q.equipment_id))
                {
                    continue;
                }
                if (q.equipment_id < 113011)
                {
                    equipment_data_list.Add(q);
                }
            }*/

            /*
             foreach (var q in equipment_data.ToList())
            {
                
                if (exf!=null)
                {
                    if (exf.drop_reward_id % 100000 / 1000 <= 32)
                    {
                        equipment_data_list.Add(q);
                    }
                }
            }
            */

            equipment_craft_list = equipment_craft.ToList();

            //unit_data_list = unit_data.ToList();
            temp = new ArrayList();
            
            foreach (var tmp in unit_data)
            {
                if (tmp.unit_id < 114701)//矛依未（新年）
                {
                    temp.Add(tmp);
                }
            }

            List<unit_data> dinosaurs = new List<unit_data>() {
                new unit_data()
                {
                    unit_id = 180401,
                    unit_name = "佩可莉姆（公主）",
                    bieming = "高达",
                    rank = 16
                },
                new unit_data()
                {
                    unit_id = 180501,
                    unit_name = "可可萝（公主）",
                    bieming = "蝶妈",
                    rank = 7
                },
                new unit_data()
                {
                    unit_id = 180201,
                    unit_name = "优衣（公主）",
                    bieming = "掉毛",
                    rank = 7
                },
                new unit_data()
                {
                    unit_id = 180101,
                    unit_name = "日和莉（公主）",
                    bieming = "火猫",
                    rank = 7
                },
            };

            temp.AddRange(
                dinosaurs
            );

            foreach (var tmp in temp)
            {
                var c = (unit_data)tmp;
                c.SetSubItems();
            }
            unit_data_list = temp.Cast<unit_data>().ToList();
            unit_datavisible = temp.Cast<unit_data>().ToList();

            unit_promotion_list = unit_promotion.ToList();

            var temp1 = new Dictionary<int, List<enemy_reward_data_List>>();
            temp2.Clear();

            foreach (var e in enemy_reward_data)
            {
                temp1.Add(e.drop_reward_id, new List<enemy_reward_data_List>()
                    {
                    new enemy_reward_data_List()
                        {
                            reward_id = e.reward_id_1,
                            odds = e.odds_1
                        },
                        new enemy_reward_data_List()
                        {
                            reward_id = e.reward_id_2,
                            odds = e.odds_2
                        },new enemy_reward_data_List()
                        {
                            reward_id = e.reward_id_3,
                            odds = e.odds_3
                        },new enemy_reward_data_List()
                        {
                            reward_id = e.reward_id_4,
                            odds = e.odds_4
                        },new enemy_reward_data_List()
                        {
                            reward_id = e.reward_id_5,
                            odds = e.odds_5
                        }
                    });

                enemy_rewardvoid(e.reward_id_1,e.drop_reward_id);
                enemy_rewardvoid(e.reward_id_2,e.drop_reward_id);
                enemy_rewardvoid(e.reward_id_3,e.drop_reward_id);
                enemy_rewardvoid(e.reward_id_4,e.drop_reward_id);
                enemy_rewardvoid(e.reward_id_5,e.drop_reward_id);


                /*enemy_reward_data_lists.Add(new enemy_reward_data_List()
                {
                    reward_id = e.reward_id_1,
                    drop_reward_id = e.drop_reward_id,
                });
                enemy_reward_data_lists.Add(new enemy_reward_data_List()
                {
                    reward_id = e.reward_id_2,
                    drop_reward_id = e.drop_reward_id,
                });
                enemy_reward_data_lists.Add(new enemy_reward_data_List()
                {
                    reward_id = e.reward_id_3,
                    drop_reward_id = e.drop_reward_id,
                });
                enemy_reward_data_lists.Add(new enemy_reward_data_List()
                {
                    reward_id = e.reward_id_4,
                    drop_reward_id = e.drop_reward_id,
                });
                enemy_reward_data_lists.Add(new enemy_reward_data_List()
                {
                    reward_id = e.reward_id_5,
                    drop_reward_id = e.drop_reward_id,
                });*/
            }


            foreach (var q in equipment_data.ToList())
            {
                //if (q.equipment_id < 113011)
                //{
                    /*var exf = enemy_reward_data_lists.Find(x => x.reward_id % 10000 == q.equipment_id % 10000);
                    if (exf != null)
                    {
                        if (exf.drop_reward_id % 100000 / 1000 <= 地图)//地图限制
                        {
                            equipment_data_list.Add(q);
                        }
                    }*/

                    if (temp2.ContainsKey(q.equipment_id))
                    {
                    equipment_data_list.Add(q);
                    var exf = temp2[q.equipment_id];
                    
                    }
                //}
            }


            
            foreach (var w in wave_group_data)
            {
                var all = new List<enemy_reward_data_List>();
                if (temp1.ContainsKey(w.drop_reward_id_1))
                {
                    all = all.Concat(temp1[w.drop_reward_id_1]).ToList();
                }
                if (temp1.ContainsKey(w.drop_reward_id_2))
                {
                    all = all.Concat(temp1[w.drop_reward_id_2]).ToList();

                }
                if (temp1.ContainsKey(w.drop_reward_id_3))
                {
                    all = all.Concat(temp1[w.drop_reward_id_3]).ToList();

                }
                if (temp1.ContainsKey(w.drop_reward_id_4))
                {
                    all = all.Concat(temp1[w.drop_reward_id_4]).ToList();
                }
                if (temp1.ContainsKey(w.drop_reward_id_5))
                {
                    all = all.Concat(temp1[w.drop_reward_id_5]).ToList();
                }
                //MessageBox.Show(all.Count + "");
                wave_group_data_lists.Add(new wave_group_data_List()
                {
                    drop_reward_id = w.drop_reward_id_1,
                    wave_group_id = w.wave_group_id,
                    /*enemy_reward_data_list = enemy_reward_data_lists.FindAll(x => 
                        x.drop_reward_id == w.drop_reward_id_1 |
                        x.drop_reward_id==w.drop_reward_id_2 |
                        x.drop_reward_id == w.drop_reward_id_3 |
                        x.drop_reward_id == w.drop_reward_id_4 |
                        x.drop_reward_id == w.drop_reward_id_5
                        )*/
                    enemy_reward_data_list = all
                });
            }

            foreach (var q in quest_data)
            {
                if ((q.area_id % 100)> 地图)//地图限制
                {
                    continue;
                }
                if (q.quest_id< 13000000)
                {
                    quest_data_lists.Add(new quest_data_List()
                    {
                        quest_id = q.quest_id,
                        areaId = q.area_id,
                        quest_name = q.quest_name,
                        wave_group_data_list = wave_group_data_lists.FindAll(x =>
                        x.wave_group_id == q.wave_group_id_1 |
                        x.wave_group_id == q.wave_group_id_2 |
                        x.wave_group_id == q.wave_group_id_3)
                    });
                }
            }
        }
    }
}
