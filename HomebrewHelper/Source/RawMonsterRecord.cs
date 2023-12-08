namespace HomebrewHelper.Source
{

    //Class meant to reflect JSON format for a monster
    public class RawMonsterRecord
    {
        public MetaData Meta { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public string Description { get; set; }
        public int? Hp { get; set; }
        public int? Ac { get; set; }
        public SaveBonuses Saves { get; set; }
        public string[] Immunities { get; set; }
        public string[] Resistances { get; set; }
        public string[] Weaknesses { get; set; }
        public AttributeBonuses Attributes { get; set; }
        public string[] Abilities { get; set; }

        public class MetaData {
            public string AonUrl { get; set;  }
            public string ImgUrl { get; set; }
        }

        public class SaveBonuses { 
            public int Fort { get; set; }
            public int Ref { get; set; }
            public int Will { get; set; }
        }

        public class AttributeBonuses { 
            public int Str { get; set; }
            public int Dex { get; set; }
            public int Con { get; set; }
            public int Int { get; set; }
            public int Wis { get; set; }
            public int Cha { get; set; }
        }
    }
}
