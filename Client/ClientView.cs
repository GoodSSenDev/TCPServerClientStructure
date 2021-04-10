using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TCPNetworkModule.Json;

namespace Client
{
    public partial class ClientUI : Form
    {
        private ClientNetworkingTool _clientNetworkingTool = new ClientNetworkingTool();

        public ClientUI()
        {
            InitializeComponent();
        }

        //This method is the click event for the connect button
        private async void button1_Click(object sender, EventArgs e)
        {
            await this._clientNetworkingTool.ConnectAsync();
            //TODO: Send message
        }

        
    }
}
