using HomebrewHelper.Source;
using Microsoft.AspNetCore.Components;

namespace HomebrewHelper.Shared
{
    public partial class MonsterEntry
    {
        [Parameter]
        public Monster monsterSource { get; set; }

        [Parameter]
        public Action<Monster> OnSelectAction { get; set; }

        public void Select() 
        {
            OnSelectAction(monsterSource);
        }
    }
}
