using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace pcr_rank_equipment_query
{
    internal class equipment_data : ListViewItem
    {
        public int equipment_id { get; set; }
        public string equipment_name { get; set; }
    }
    internal class equipment_data_View : ListViewItem
    {
        public int equipment_id { get; set; }
        public string equipment_name { get; set; }
        public int consume_num_1 { get; set; }
        public void SetSubItems()
        {
            Text = equipment_name;
            SubItems.AddRange(new[]
            {
                consume_num_1.ToString(),
                equipment_id.ToString()
            });
            //MessageBox.Show(equipment_name);
        }
    }
    internal class unit_data : ListViewItem
    {
        public int unit_id { get; set; }
        public string unit_name { get; set; }
        public string bieming { get; set; }

        public void SetSubItems()
        {
            Text = unit_name;
            SubItems.AddRange(new[]
            {
                bieming,
                unit_id.ToString(),
                unit_name
            });
        }
    }
    internal class unit_promotion
    {
        public int unit_id { get; set; }
        public int promotion_level { get; set; }
        public int equip_slot_1 { get; set; }
        public int equip_slot_2 { get; set; }
        public int equip_slot_3 { get; set; }
        public int equip_slot_4 { get; set; }
        public int equip_slot_5 { get; set; }
        public int equip_slot_6 { get; set; }
    }
    internal class equipment_craft
    {
        public int equipment_id { get; set; }
        public int condition_equipment_id_1 { get; set; }
        public int consume_num_1 { get; set; }
        public int condition_equipment_id_2 { get; set; }
        public int consume_num_2 { get; set; }
        public int condition_equipment_id_3 { get; set; }
        public int consume_num_3 { get; set; }
        public int condition_equipment_id_4 { get; set; }
        public int consume_num_4 { get; set; }
    }
    internal class quest_data
    {
        public int quest_id { get; set; }
        public int area_id { get; set; }
        public string quest_name { get; set; }
        public int wave_group_id_1 { get; set; }
        public int wave_group_id_2 { get; set; }
        public int wave_group_id_3 { get; set; }
    }
    internal class quest_data_List : ListViewItem
    {
        public int quest_id { get; set; }
        public int areaId { get; set; }
        public string quest_name { get; set; }
        public int odds { get; set; }
        public Color color { get; set; }
        public List<wave_group_data_List> wave_group_data_list { get; set; }

        public void SetSubItems()
        {
            Text = questType(areaId) + " " + quest_name;
            BackColor = color;
            SubItems.AddRange(new[]
            {
                odds.ToString()+"%",
                quest_name
            });
        }
        public string questType(int areaId)
        {
            string Type;
            if (areaId > 11000 & areaId < 11999)
            {
                Type = "Normal";
            }
            else if (areaId > 12000 & areaId < 12999)
            {
                Type = "Hard";
            }
            else if (areaId > 13000 & areaId < 13999)
            {
                Type = "VeryHard";
            }
            else
            {
                Type = "Others";
            }
            return Type;
        }
    }

    #region Quest old

    internal class wave_group_data
    {
        public int wave_group_id { get; set; }
        public int drop_reward_id_1 { get; set; }
        public int drop_reward_id_2 { get; set; }
        public int drop_reward_id_3 { get; set; }
        public int drop_reward_id_4 { get; set; }
        public int drop_reward_id_5 { get; set; }
    }
    internal class wave_group_data_List
    {
        public int wave_group_id { get; set; }
        public int drop_reward_id { get; set; }
        public List<enemy_reward_data_List> enemy_reward_data_list { get; set; }
    }
    
    #endregion

    internal class enemy_reward_data
    {
        public int drop_reward_id { get; set; }
        public int reward_id_1 { get; set; }
        public int odds_1 { get; set; }
        public int reward_id_2 { get; set; }
        public int odds_2 { get; set; }
        public int reward_id_3 { get; set; }
        public int odds_3 { get; set; }
        public int reward_id_4 { get; set; }
        public int odds_4 { get; set; }
        public int reward_id_5 { get; set; }
        public int odds_5 { get; set; }
    }

    internal class enemy_reward_data_List
    {
        public int drop_reward_id { get; set; }
        public int reward_id { get; set; }
        public int odds { get; set; }
    }
}
