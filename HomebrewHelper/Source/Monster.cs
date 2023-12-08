using System.Runtime.ExceptionServices;
using System.Text.RegularExpressions;

namespace HomebrewHelper.Source
{
    //Class used as the "point" in the KNN cloud
    public class Monster
    {
        public string SourceURL { get; set; } = "";
        public string ImgUrl { get; set; } = "";

        public string Name { get; set; } = "Unnamed Monster";
        public string Description { get; set; } = "";
        public int Level { get; set; } = 0;

        public int?[] Hyperposition { get; set; } = new int?[0];

        public List<string> ActionText { get; set; } = new List<string>();

        //Initializes this from a RarMonsterRecord directly
        public Monster FromRawMonsterRecord(RawMonsterRecord record) 
        {   
            this.SourceURL = record.Meta.AonUrl;
            this.ImgUrl = record.Meta.ImgUrl;
            this.Name = record.Name;
            this.Description = record.Description;
            this.Level = record.Level;

            int? hp = record.Hp;
            int? ac = record.Ac;
            int?[] attr = {
                record.Attributes.Str, 
                record.Attributes.Dex, 
                record.Attributes.Con, 
                record.Attributes.Int, 
                record.Attributes.Wis, 
                record.Attributes.Cha
            };
            int?[] saves = {
                record.Saves.Fort,
                record.Saves.Ref,
                record.Saves.Will,
            };
            int imm = record.Immunities.Length;
            int weak = record.Weaknesses.Length;
            int res = record.Resistances.Length;
            int abilCount = record.Abilities.Length;

            int? avgDmg = null;
            foreach (string ability in record.Abilities) 
            {
                ActionText.Add(ability);
                if (ability.Contains("[1 Action]"))
                {
                    Match match = Regex.Match(ability, "[0-9]+d[0-9]+(\\+[0-9]+)?");
                    if (match.Success) 
                    {
                        float multiplier = float.Parse(match.Value.Split("d")[0]);
                        string[] diceSuffix = match.Value.Split("d")[1].Split("+");
                        float diceValue = float.Parse(diceSuffix[0]);
                        float damageAddition = diceSuffix.Length > 1 ? float.Parse(diceSuffix[1]) : 0;
                        float average = (((multiplier * diceValue) + multiplier) / 2) + damageAddition;
                        avgDmg = (average > avgDmg || avgDmg == null) ? (int)average : avgDmg;
                    }
                }
            }

            this.Hyperposition = new int?[16] { hp, ac, attr[0], attr[1], attr[2], attr[3], attr[4], attr[5], saves[0], saves[1], saves[2], imm, weak, res, abilCount, avgDmg };
            return this;
        }
    }
}
