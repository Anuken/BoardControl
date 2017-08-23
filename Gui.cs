using System;
using Gtk;

namespace BoardControl{
	
	public class Gui{
		//Controller controller = Controllers.riskOfRain;
		ControlData data;
		BoardInput input;
		string status = "Disconnected";

		Window win;
		Label statuslabel;

		public static void Main(string[] args){
			new Gui ();
		}

		public Gui (){
			data = new ControlData ();
			//controller (data);

			input = new BoardInput ();
			input.connected += (sender, e) => {
				UpdateStatus("Connected");
			};

			input.disconnected += (sender, e) => {
				UpdateStatus("Disconnected");
			};

			Application.Init ();

			SetupUI ();

			Application.Run ();
		}

		void Connect(){
			Console.WriteLine ("connecting");
			UpdateStatus("Connecting...");
			input.Pair (data);
		}

		void Disconnect(){
			UpdateStatus("Disconnecting...");
			input.Disconnect ();
		}

		void UpdateData(){
			input.data = data;
		}

		void UpdateStatus(string newstatus){
			status = newstatus;
			statuslabel.Text = newstatus;
			statuslabel.QueueDraw ();
		}

		void SetupUI(){
			win = new Window("Board Control");
			win.Resize(600, 550);

			VBox box = new VBox ();

			HBox guide = new HBox ();
			guide.Add (new Label("Event"));
			guide.Add (new Label("Type"));
			guide.Add (new Label("Key"));
			guide.Add (new Label(""));

			box.PackStart (guide, false, false, 0);

			AddBox (box);

			Button addButton = new Button ("+ New Control");
			addButton.Pressed += (o, args) => {
				AddBox (box);
				box.QueueDraw();
				win.ShowAll();
			};

			VBox framebox = new VBox ();

			framebox.Add (statuslabel = new Label(status){
				Margin = 10
			});

			HBox buttonbox = new HBox (){
				Margin = 4
			};
			Button connect = new Button ("Connect");
			Button disconnect = new Button ("Disconnect");
			connect.Pressed += (o, args) => { Connect(); };
			disconnect.Pressed += (o, args) => { Disconnect(); };
			buttonbox.Add (connect);
			buttonbox.Add (disconnect);
			framebox.Add (buttonbox);


			Frame frame = new Frame (){
				Margin = 4, BorderWidth = 6
			};
			frame.Add (framebox);

			box.PackEnd (frame, false, false, 0);

			box.PackEnd (addButton, false, false, 0);

			win.Add(box);

			win.ShowAll();
		}

		void AddBox(VBox box){
			HBox side = new HBox ();


			ComboBoxText eventbox = new ComboBoxText ();

			foreach(Pad pad in input.pads){
				eventbox.AppendText (pad.name);
			}

			eventbox.Active = 0;

			ComboBoxText typebox = new ComboBoxText ();

			foreach(EventType type in data.keys.Keys){
				typebox.AppendText (type.ToString());
			}

			typebox.Active = 0;

			ComboBoxText keybox = new ComboBoxText ();

			EventHandler changed = (e, args) => {
				data.keys[(EventType)typebox.Active][eventbox.Active] = Keys.All[keybox.Active];
			};

			keybox.Changed += changed;
			typebox.Changed += changed;
			eventbox.Changed += changed;

			foreach(Key key in Keys.All){
				keybox.AppendText (key.name);
			}

			keybox.Active = 0;

			Button removeButton = new Button ("- Remove");
			removeButton.Pressed += (sender, e) => {
				data.keys[(EventType)typebox.Active][eventbox.Active] = null;
				box.Remove(side);
				win.ShowAll();
			};

			side.Add (eventbox);
			side.Add (typebox);
			side.Add (keybox);
			side.Add (removeButton);

			box.PackStart (side, false, false, 0);

		}

		ComboBox CreateCombo(EventHandler handler){
			ComboBoxText combo = new ComboBoxText ();
			combo.Changed += handler;

			combo.AppendText ("<none>");

			foreach(Key key in Keys.All){
				combo.AppendText (key.name);
			}

			combo.Active = 0;

			return combo;
		}
	}

}

