using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileExplorer
{
    public class ItemDataUsers
    {
        public string Name { get; set; }
        public string Password { get; set; }

        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
    }
}
