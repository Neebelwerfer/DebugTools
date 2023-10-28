using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.UI
{
    public enum ClickType
    {
        Up,
        Hold,
        Down
    }

    internal interface IUIMenu
    {
        string DisplayName();
        bool HasData();
        void OnGUI();
        void OnClick(ClickType type, int mouse, bool MenuClicked);
        bool Back();
        void Exit();
    }
}
