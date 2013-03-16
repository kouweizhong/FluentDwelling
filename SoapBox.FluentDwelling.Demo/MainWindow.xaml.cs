using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using SoapBox.FluentDwelling.Devices;
using System.Threading;

namespace SoapBox.FluentDwelling.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Plm plm = new Plm("COM4");

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Captures any errors and displays the exception message
            this.plm.OnError += new EventHandler((s, args) =>
            {
                this.ErrorMessage.Text = this.plm.Exception.Message;
            });

            // Hook into events and write a message when anything happens
            this.plm.SetButton.Tapped += new EventHandler((s, args) =>
            {
                addEventMessage("PLM SET Button Tapped.");
            });

            this.plm.SetButton.PressedAndHeld += new EventHandler((s, args) =>
            {
                addEventMessage("PLM SET Button Pressed and Held.");
            });

            this.plm.SetButton.ReleasedAfterHolding += new EventHandler((s, args) =>
            {
                addEventMessage("PLM SET Button Released After Holding.");
            });

            this.plm.SetButton.UserReset += new EventHandler((s, args) =>
            {
                addEventMessage("PLM SET Button Held During Power-up: PLM Reset.");
                readAllLinkDatabase(); // it's most likely empty...
            });

            this.plm.Network.AllLinkingCompleted += new AllLinkingCompletedHandler((s, args) =>
            {
                addEventMessage("All-Linking Completed: " + args.AllLinkingAction.ToString() + ", device: " + args.PeerId.ToString());
                readAllLinkDatabase();
            });

            this.plm.Network.StandardMessageReceived += new StandardMessageReceivedHandler((s, args) =>
            {
                var command = args.Description;
                if (command == string.Empty)
                {
                    command = "0x" + args.Command1.ToString("X") +
                        ",0x" + args.Command2.ToString("X");
                }
                addEventMessage("Standard Message Received: command=" + 
                    command + ", type=" +
                    args.MessageType.ToString() + ", group=" +
                    args.Group.ToString() + ", from: " + args.PeerId.ToString());
            });

            this.plm.Network.X10.UnitAddressed += new X10UnitAddressedHandler((s, args) =>
            {
                addEventMessage("X10 Unit Addressed: House Code " + args.HouseCode + ", Unit Code: " + args.UnitCode.ToString());
            });

            this.plm.Network.X10.CommandReceived += new X10CommandReceivedHandler((s, args) =>
            {
                addEventMessage("X10 Command Received: House Code " + args.HouseCode + ", Command: " + args.Command.ToString());
            });

            // Setup a timer on the GUI thread to call the Receive loop
            var timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100); // milliseconds
            timer.Tick += new EventHandler((s, args) =>
            {
                this.plm.Receive();
            });
            timer.Start();

            readAllLinkDatabase();
        }

        private void readAllLinkDatabase()
        {
            // Grab a list of all linked devices and display them.
            var database = this.plm.GetAllLinkDatabase();
            this.Database.Items.Clear();
            if (!this.plm.Error)
            {
                foreach (var record in database.Records)
                {
                    displayAllLinkRecord(record);
                }
            }
        }

        private void displayAllLinkRecord(PlmAllLinkDatabaseRecord record)
        {
            var master = record.PlmIsMaster ? "Master" : "Slave";
            var message = record.DeviceId.ToString() + " (Group " + record.AllLinkGroup + ") " + master;
            DeviceBase device;
            // Attempt to connect to the device to see what it is
            if (this.plm.Network.TryConnectToDevice(record.DeviceId, out device))
            {
                message = message + ": " + device.DeviceCategory + ", " + device.DeviceSubcategory;
            }
            else
            {
                message = message + ": Could not connect.";
            }
            this.Database.Items.Add(message);
        }

        private void addEventMessage(string message)
        {
            const int MESSAGE_QUEUE_LENGTH = 8;

            var messageWithTimestamp = DateTime.Now.ToString("HH:mm:ss") + " " + message;
            this.Events.Items.Insert(0, messageWithTimestamp); // insert at top
            while (this.Events.Items.Count > MESSAGE_QUEUE_LENGTH) // remove old messages
            {
                this.Events.Items.RemoveAt(MESSAGE_QUEUE_LENGTH);
            }
        }

        private void X10A2On_Click(object sender, RoutedEventArgs e)
        {
            this.plm.Network.X10
                .House("A")
                .Unit(2)
                .Command(X10Command.On);
        }

        private void X10A2Off_Click(object sender, RoutedEventArgs e)
        {
            this.plm.Network.X10
                .House("A")
                .Unit(2)
                .Command(X10Command.Off);
        }

        private void X10AAllOn_Click(object sender, RoutedEventArgs e)
        {
            this.plm.Network.X10
                .House("A")
                .Command(X10Command.AllLightsOn);
        }

        private void X10AAllOff_Click(object sender, RoutedEventArgs e)
        {
            this.plm.Network.X10
                .House("A")
                .Command(X10Command.AllUnitsOff); // there is an AllLightsOff command, but not all older lamp modules respond to it :)
        }

        private void X10StatusA2_Click(object sender, RoutedEventArgs e)
        {
            this.plm.Network.X10
                .House("A")
                .Unit(2)
                .Command(X10Command.StatusRequest); // this doesn't work on all X10 devices, but the device should respond
        }
    }
}
