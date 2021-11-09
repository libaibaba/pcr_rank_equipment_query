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
        static string DBfile = Path.Combine(Path.GetTempPath(), "redive_cn.db");
        public static ArrayList temp = new ArrayList();
        public static List<equipment_data> equipment_data_list = new List<equipment_data>();
        public static List<unit_data> unit_data_list = new List<unit_data>();
        public static List<unit_promotion> unit_promotion_list = new List<unit_promotion>();
        public static List<equipment_craft> equipment_craft_list = new List<equipment_craft>();
        public static List<quest_data> quest_data_list = new List<quest_data>();
        public static List<enemy_reward_data> enemy_reward_data_list = new List<enemy_reward_data>();
        public static List<wave_group_data> wave_group_data_list = new List<wave_group_data>();
        


        public static List<unit_data> unit_datavisible = new List<unit_data>();
        public static List<quest_data_List> quest_data_lists = new List<quest_data_List>();
        public static List<enemy_reward_data_List> enemy_reward_data_lists = new List<enemy_reward_data_List>();
        public static List<wave_group_data_List> wave_group_data_lists = new List<wave_group_data_List>();


        public static string SaveDB()
        {
            if (!File.Exists(DBfile))
            {
                File.WriteAllBytes(DBfile, Properties.Resources.redive_cn);
            }
            LoadDB();
            return "ok";
        }
        public static void LoadDB()
        {
            var DBFile = new SQLiteConnection(@"URI=file:" + DBfile);
            //SqlDataAdapter adapter = new SqlDataAdapter("select *,case Gender when'0' then '男' else '女' end as 性别 form 表名", conn);
            //var DBFile = new SQLiteConnection(@"URI=file:G:\原神\redive_cn.db");
            var equipment_data = DBFile.Query<equipment_data>("SELECT equipment_id,equipment_name FROM equipment_data", new DynamicParameters());
            var unit_data = DBFile.Query<unit_data>("SELECT unit_id,unit_name,bieming FROM unit_data_zh", new DynamicParameters());
            var unit_promotion = DBFile.Query<unit_promotion>("SELECT unit_id,promotion_level,equip_slot_1,equip_slot_2,equip_slot_3,equip_slot_4,equip_slot_5,equip_slot_6 FROM unit_promotion", new DynamicParameters());
            var equipment_craft = DBFile.Query<equipment_craft>("SELECT equipment_id,condition_equipment_id_1,consume_num_1,condition_equipment_id_2,consume_num_2,condition_equipment_id_3,consume_num_3,condition_equipment_id_4,consume_num_4 FROM equipment_craft", new DynamicParameters());
            var quest_data = DBFile.Query<quest_data>("SELECT quest_id,area_id,quest_name,wave_group_id_1,wave_group_id_2,wave_group_id_3,reward_image_1,reward_image_2,reward_image_3,reward_image_4,reward_image_5 FROM quest_data", new DynamicParameters());
            var enemy_reward_data = DBFile.Query<enemy_reward_data>("SELECT drop_reward_id,reward_id_1,odds_1,reward_id_2,odds_2,reward_id_3,odds_3,reward_id_4,odds_4,reward_id_5,odds_5 FROM enemy_reward_data", new DynamicParameters());
            var wave_group_data = DBFile.Query<wave_group_data>("SELECT wave_group_id,drop_reward_id_1,drop_reward_id_2,drop_reward_id_3,drop_reward_id_4,drop_reward_id_5 FROM wave_group_data", new DynamicParameters());

            equipment_data_list = equipment_data.ToList();
            equipment_craft_list = equipment_craft.ToList();

            unit_data_list = unit_data.ToList();
            temp = new ArrayList();
            foreach (var tmp in unit_data)
            {
                if (tmp.unit_id < 400000)
                {
                    temp.Add(tmp);
                }
            }
            foreach (var tmp in temp)
            {
                var c = (unit_data)tmp;
                c.SetSubItems();
            }
            unit_data_list = temp.Cast<unit_data>().ToList();
            unit_datavisible = temp.Cast<unit_data>().ToList();

            unit_promotion_list = unit_promotion.ToList();

            foreach (var e in enemy_reward_data)
            {
                enemy_reward_data_lists.Add(new enemy_reward_data_List()
                {
                    reward_id = e.reward_id_1,
                    drop_reward_id = e.drop_reward_id,
                    odds = e.odds_1
                });
                enemy_reward_data_lists.Add(new enemy_reward_data_List()
                {
                    reward_id = e.reward_id_2,
                    drop_reward_id = e.drop_reward_id,
                    odds = e.odds_2
                });
                enemy_reward_data_lists.Add(new enemy_reward_data_List()
                {
                    reward_id = e.reward_id_3,
                    drop_reward_id = e.drop_reward_id,
                    odds = e.odds_3
                });
                enemy_reward_data_lists.Add(new enemy_reward_data_List()
                {
                    reward_id = e.reward_id_4,
                    drop_reward_id = e.drop_reward_id,
                    odds = e.odds_4
                });
                enemy_reward_data_lists.Add(new enemy_reward_data_List()
                {
                    reward_id = e.reward_id_5,
                    drop_reward_id = e.drop_reward_id,
                    odds = e.odds_5
                });
            }

            foreach (var w in wave_group_data)
            {
                wave_group_data_lists.Add(new wave_group_data_List()
                {
                    drop_reward_id = w.drop_reward_id_1,
                    wave_group_id = w.wave_group_id,
                    enemy_reward_data_list = enemy_reward_data_lists.FindAll(x => 
                        x.drop_reward_id == w.drop_reward_id_1 |
                        x.drop_reward_id==w.drop_reward_id_2 |
                        x.drop_reward_id == w.drop_reward_id_3 |
                        x.drop_reward_id == w.drop_reward_id_4 |
                        x.drop_reward_id == w.drop_reward_id_5
                        )
                });
            }

            foreach (var q in quest_data)
            {
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
                        x.wave_group_id == q.wave_group_id_3
                    )
                    });
                }
            }
        }
    }
}
