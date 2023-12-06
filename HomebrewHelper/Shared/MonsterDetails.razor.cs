using HomebrewHelper.Source;
using Microsoft.AspNetCore.Components;

namespace HomebrewHelper.Shared
{
    public partial class MonsterDetails
    {
        [Parameter]
        public Monster monsterSource { get; set; }
    }
}
