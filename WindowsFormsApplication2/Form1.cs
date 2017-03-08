using System;
using Sulakore.Communication;
using Tangine;
using GlobalLowLevelHooks;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Form1 : ExtensionForm
    {
        public override bool IsRemoteModule { get; } = true;

        private ushort _move_furni_header;

        private KeyboardHook hook = new KeyboardHook();

        public Form1()
        {
            InitializeComponent();

            FormClosing += formHasBeenClosed;

            Triggers.OutAttach(1987, clickedOnAHabbo);

            // i intercept all the "click on tile" headers
            Triggers.OutAttach(2255, clickedOnATile);

            _move_furni_header = Game.GetHeader("1101e72b4882377d9dc313cfa46d6d3d");

            hook.KeyDown += new KeyboardHook.KeyboardHookCallback(thisMethodGetsFiredOnKeyDown);
            hook.Install();
        }

        private void clickedOnATile(DataInterceptedEventArgs obj)
        {
            if (checkBox1.Checked)
            {
                // here i say, i block the packet, so it won't be sent to the server
                obj.IsBlocked = true;

                int x = obj.Packet.ReadInteger();
                int y = obj.Packet.ReadInteger();

                // packet to move the furni
                Connection.SendToServerAsync(_move_furni_header, 378994020, x, y, 0);

                // packet to move to a tile
                Connection.SendToServerAsync(2255, x, y);
            }
        }

        private void clickedOnAHabbo(DataInterceptedEventArgs obj)
        {
            int clicked_habbo_id = obj.Packet.ReadInteger();
            
            Invoke(new MethodInvoker(delegate ()
            {
                label2.Text = clicked_habbo_id.ToString();
            }));
        }

        private void thisMethodGetsFiredOnKeyDown(KeyboardHook.VKeys key)
        {
            if (key == KeyboardHook.VKeys.KEY_1)
            {
                Connection.SendToServerAsync(3182, 43427677);
            }
        }

        private void formHasBeenClosed(object sender, FormClosingEventArgs e)
        {
            hook.Uninstall();
        }
        
    }
}
