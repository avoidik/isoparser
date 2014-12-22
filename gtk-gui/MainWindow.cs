
// This file has been generated by the GUI designer. Do not modify.

public partial class MainWindow
{
	private global::Gtk.VBox vbox3;
	
	private global::Gtk.ScrolledWindow GtkScrolledWindow;
	
	private global::Gtk.TextView textviewLog;
	
	private global::Gtk.HBox hbox1;
	
	private global::Gtk.Button buttonLoadYaml;
	
	private global::Gtk.Button buttonProcess;

	protected virtual void Build ()
	{
		global::Stetic.Gui.Initialize (this);
		// Widget MainWindow
		this.WidthRequest = 500;
		this.HeightRequest = 300;
		this.Name = "MainWindow";
		this.Title = global::Mono.Unix.Catalog.GetString ("IsoParser [ISO-3166]");
		this.WindowPosition = ((global::Gtk.WindowPosition)(1));
		this.Resizable = false;
		this.AllowGrow = false;
		// Container child MainWindow.Gtk.Container+ContainerChild
		this.vbox3 = new global::Gtk.VBox ();
		this.vbox3.Name = "vbox3";
		this.vbox3.Spacing = 6;
		// Container child vbox3.Gtk.Box+BoxChild
		this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
		this.GtkScrolledWindow.Name = "GtkScrolledWindow";
		this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
		this.textviewLog = new global::Gtk.TextView ();
		this.textviewLog.CanFocus = true;
		this.textviewLog.Name = "textviewLog";
		this.textviewLog.Editable = false;
		this.GtkScrolledWindow.Add (this.textviewLog);
		this.vbox3.Add (this.GtkScrolledWindow);
		global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.GtkScrolledWindow]));
		w2.Position = 0;
		// Container child vbox3.Gtk.Box+BoxChild
		this.hbox1 = new global::Gtk.HBox ();
		this.hbox1.Name = "hbox1";
		this.hbox1.Spacing = 6;
		// Container child hbox1.Gtk.Box+BoxChild
		this.buttonLoadYaml = new global::Gtk.Button ();
		this.buttonLoadYaml.CanFocus = true;
		this.buttonLoadYaml.Name = "buttonLoadYaml";
		this.buttonLoadYaml.UseUnderline = true;
		this.buttonLoadYaml.Label = global::Mono.Unix.Catalog.GetString ("Load YAML");
		this.hbox1.Add (this.buttonLoadYaml);
		global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.buttonLoadYaml]));
		w3.Position = 0;
		w3.Expand = false;
		w3.Fill = false;
		// Container child hbox1.Gtk.Box+BoxChild
		this.buttonProcess = new global::Gtk.Button ();
		this.buttonProcess.CanFocus = true;
		this.buttonProcess.Name = "buttonProcess";
		this.buttonProcess.UseUnderline = true;
		this.buttonProcess.Label = global::Mono.Unix.Catalog.GetString ("Process data");
		this.hbox1.Add (this.buttonProcess);
		global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.buttonProcess]));
		w4.Position = 1;
		w4.Expand = false;
		w4.Fill = false;
		this.vbox3.Add (this.hbox1);
		global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.hbox1]));
		w5.Position = 1;
		w5.Expand = false;
		w5.Fill = false;
		this.Add (this.vbox3);
		if ((this.Child != null)) {
			this.Child.ShowAll ();
		}
		this.DefaultWidth = 500;
		this.DefaultHeight = 300;
		this.Show ();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
		this.buttonLoadYaml.Clicked += new global::System.EventHandler (this.OnButtonLoadYamlClicked);
		this.buttonProcess.Clicked += new global::System.EventHandler (this.OnButtonProcessClicked);
	}
}
