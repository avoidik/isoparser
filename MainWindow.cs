using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Gtk;
using IsoParserHelper;
using CSharpTree;

public partial class MainWindow: Gtk.Window
{
	private string strFilepath;

	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void OnButtonLoadYamlClicked (object sender, EventArgs e)
	{
		FileChooserDialog fcd = new FileChooserDialog (
			"Please select yaml file",
			this,
			FileChooserAction.Open,
			"Cancel",ResponseType.Cancel,
			"Open",ResponseType.Accept
		);

		if (fcd.Run () == (int)ResponseType.Accept)	{
			strFilepath = fcd.Filename;
			UtilsClass.DoLog (textviewLog,
				"- Selected file " + strFilepath + Environment.NewLine + "- Awaiting orders..."
			);
		}

		fcd.Destroy ();
	}

	protected void OnButtonProcessClicked (object sender, EventArgs e)
	{
		if (String.IsNullOrEmpty (strFilepath)) {
			UtilsClass.DoLog (textviewLog, "- Select file first.");
			this.OnButtonLoadYamlClicked (sender, e);
			return;
		}

		var listCountry = new List<LangCountryClass> ();
		var listRegion = new List<LangRegionClass> ();

		// read file and populate list

		UtilsClass.DoLog (textviewLog, "- Let's begin...");

		StreamReader file = new StreamReader (strFilepath);
		String strLine = String.Empty;
		Match match;

		while ((strLine = file.ReadLine ()) != null) {
			// country.XX: YYY
			// region.XX-XX: YYY
			match = Regex.Match(strLine, @"^(country|region)+\.(?<short_name>[A-Za-z0-9\-]+)\:\s(?<full_name>.*)$",RegexOptions.IgnoreCase);
			if (match.Success) {
				if (match.Groups [1].Value.ToLower () == "region") {
					listRegion.Add (new LangRegionClass { 
						FullName = match.Groups ["full_name"].Value,
						ShortName = match.Groups ["short_name"].Value,
						TranslatedAs = "<none>",
						Parent = null
					});
				} else if (match.Groups [1].Value.ToLower () == "country") {
					listCountry.Add (new LangCountryClass { 
						FullName = match.Groups ["full_name"].Value,
						ShortName = match.Groups ["short_name"].Value,
						TranslatedAs = "<none>",
						MyTree = null
					});
				} else {
					UtilsClass.DoLog (textviewLog, "- Something wrong with regex pattern, check it please.");
					break;
				}
			}
		}
		file.Close();

		// build tree

		foreach (LangCountryClass lcc in listCountry) {

			lcc.MyTree = new CSharpTree.TreeNode<LangRegionClass> (null);

			foreach (LangRegionClass lrc in listRegion) {

				if (lcc.ShortName == lrc.ShortName.Substring (0, 2)) {

					lcc.MyTree.AddChild (lrc);
					lrc.Parent = lcc;

				}

			}

		}

		int CountOrph = listRegion.Where(p => p.Parent == null).Count();
		if (CountOrph != 0) {
			foreach(LangRegionClass lrc in listRegion) {
				if (lrc.Parent == null) {
					UtilsClass.DoLog (textviewLog, "- Orphaned item: {0} [{1}]", lrc.FullName, lrc.ShortName);
				}
			}
		}

		// contact wiki

		UtilsClass.DoLog (textviewLog, "- We have {0} countries and {1} regions in this file...", listCountry.Count, listRegion.Count);

		//
		// request "ISO 3166-2:XX" where XX country code
		//

		string strBaseUrl = @"http://ru.wikipedia.org/wiki/ISO_3166-2";
		string strFullUrl = @"http://ru.wikipedia.org/wiki/ISO_3166-2:{0}";
		string strFullISO = "ISO 3166-2:{0}";
		string strBasePage = string.Empty;
		int hCode = 0;

		// get base page

		hCode = UtilsClass.RetrievePage (strBaseUrl, out strBasePage);
		if (strBasePage.Length == 0 || hCode != 200) {
			UtilsClass.DoLog (textviewLog, "- Error while base wiki-page retrieving. Over.");
			return;
		}

		// check for each country

		foreach (LangCountryClass lcc in listCountry) {

			string strCurrent = string.Format (strFullISO, lcc.ShortName);

			if (strBasePage.Contains (strCurrent) == false) {
				UtilsClass.DoLog (textviewLog, "- No such item " + lcc.ShortName + ". Skipped...");
				continue;
			}

			// enrich country one-by-one

			string strPage = string.Format(strFullUrl, lcc.ShortName);

			UtilsClass.EnrichDataFromPage (lcc, strPage);
		}

		// dont forget to translate countries

		UtilsClass.EnrichDataFromData (listCountry, strBasePage);

		// list untranslated regions

		foreach(LangRegionClass lrc in listRegion) {
			if (lrc.TranslatedAs == "<none>") {
				UtilsClass.DoLog (textviewLog, "- Untranslated region: {0} [{1}]", lrc.FullName, lrc.ShortName);
			}
		}

		foreach(LangCountryClass lcc in listCountry) {
			if (lcc.TranslatedAs == "<none>") {
				UtilsClass.DoLog (textviewLog, "- Untranslated country: {0} [{1}]", lcc.FullName, lcc.ShortName);
			}
		}

		// export data

		string directoryName = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
		directoryName = System.IO.Path.Combine (directoryName, "data.yml");

		if (File.Exists (directoryName)) {
			File.Delete (directoryName);
		}

		using (FileStream fs = new FileStream (directoryName, FileMode.Append, FileAccess.Write)) {
			using (StreamWriter sw = new StreamWriter(fs))
			{
				foreach (LangCountryClass lcc in listCountry) {

					sw.Write ("country." + lcc.ShortName + ": " + lcc.TranslatedAs + Environment.NewLine);

					foreach (CSharpTree.TreeNode<LangRegionClass> lrc in lcc.MyTree) {

						if (lrc.IsRoot) {
							continue;
						}

						sw.Write ("region." + lrc.Data.ShortName + ": " + lrc.Data.TranslatedAs + Environment.NewLine);
					}
				}

			}
		}

		// check un-translated data

		UtilsClass.DoLog (textviewLog, "- All done!");

	}
}
