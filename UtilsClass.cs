using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gtk;
using CSharpTree;
using HtmlAgilityPack;

namespace IsoParserHelper
{
	public class LangBaseClass {
		public string FullName { get; set; }
		public string ShortName { get; set; }
		public string TranslatedAs { get; set; }
	}

	public class LangCountryClass : LangBaseClass {
		public CSharpTree.TreeNode<LangRegionClass> MyTree { get; set; }
	}

	public class LangRegionClass : LangBaseClass {
		public LangCountryClass Parent { get; set; }
	}

	public class UtilsClass
	{
		public static void DoLog(TextView tv,string strText)
		{
			tv.Buffer.Text = (tv.Buffer.Text.Length == 0) ? strText + Environment.NewLine : tv.Buffer.Text + strText + Environment.NewLine;
		}

		public static void DoLog(TextView tv,string strText, params object[] args)
		{
			strText = String.Format (strText, args);
			tv.Buffer.Text = (tv.Buffer.Text.Length == 0) ? strText + Environment.NewLine : tv.Buffer.Text + strText + Environment.NewLine;
		}

		public static string RetrievePage(string strUrl)
		{
			int hcode;
			return RetrievePage (strUrl, out hcode);
		}

		public static int RetrievePage(string strUrl, out string strData)
		{
			int hcode;
			strData = RetrievePage (strUrl, out hcode);
			return hcode;
		}

		public static string RetrievePage(string strUrl, out int hCode)
		{
			string html = string.Empty;
			int code = 0;

			try	{
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();
				Stream data = response.GetResponseStream();
				using (StreamReader sr = new StreamReader(data))
				{
					html = sr.ReadToEnd();
				}
				code = (int)response.StatusCode;
			}
			catch {
				html = string.Empty;
			}
			finally {
				hCode = code;
			}

			return html;
		}

		public static void EnrichDataFromData(List<LangCountryClass> lccFull, string strData)
		{
			HtmlDocument hdoc = new HtmlDocument ();
			hdoc.LoadHtml (strData);

			var listTranslated = new List<LangBaseClass> ();

			string strLabel = string.Empty;
			string strTranslated = string.Empty;

			try	{
				foreach (HtmlNode table in hdoc.DocumentNode.SelectNodes("//table[contains(@class,'wikitable')]")) {
					foreach (HtmlNode row in table.SelectNodes("tr").Skip(1)) {

						int hColumn = 0;
						strLabel = string.Empty;
						strTranslated = string.Empty;

						foreach (HtmlNode cell in row.SelectNodes("th|td")) {
							if (hColumn == 0) {
								strLabel = HttpUtility.HtmlDecode (cell.InnerText);
							} else if (hColumn == 1) {
								if(cell.Descendants ("a").Count () > 0) {

									foreach(HtmlNode node in cell.Descendants ("a")) {
										string strNodeText = HttpUtility.HtmlDecode (node.InnerText);
										if(strNodeText.Contains("</img>") == false)
										{
											strTranslated = strNodeText;
										}
									}

								} else {
									strTranslated = HttpUtility.HtmlDecode (cell.InnerText);
								}
							} else {
								break;
							}
							hColumn++;
						}

						listTranslated.Add (new LangBaseClass {
							ShortName = strLabel,
							TranslatedAs = strTranslated
						});
					}
				}
			}
			catch(Exception e) {
				strData = "Error " + e.Message + ". " + strLabel + " " + strTranslated;
			}

			foreach (LangCountryClass lcc in lccFull) {

				// lrc.Data.ShortName

				foreach (LangBaseClass lbc in listTranslated) {

					if ("ISO 3166-2:" + lcc.ShortName == lbc.ShortName) {

						lcc.TranslatedAs = lbc.TranslatedAs;

					}

				}

			}
		}

		public static void EnrichDataFromPage(LangCountryClass lcc, string strUrl)
		{
			string strData;
			int hCode;

			hCode = UtilsClass.RetrievePage( strUrl, out strData);
			if (strData.Length == 0 || hCode != 200) {
				return;
			}

			HtmlDocument hdoc = new HtmlDocument ();
			hdoc.LoadHtml (strData);

			var listTranslated = new List<LangBaseClass> ();

			string strLabel = string.Empty;
			string strTranslated = string.Empty;

			try	{
				foreach (HtmlNode table in hdoc.DocumentNode.SelectNodes("//table[contains(@class,'wikitable')]")) {
					foreach (HtmlNode row in table.SelectNodes("tr").Skip(1)) {

						int hColumn = 0;
						strLabel = string.Empty;
						strTranslated = string.Empty;

						foreach (HtmlNode cell in row.SelectNodes("th|td")) {
							if (hColumn == 0) {
								strLabel = HttpUtility.HtmlDecode (cell.InnerText);
							} else if (hColumn == 1) {
								if(cell.Descendants ("a").Count () > 0) {

									foreach(HtmlNode node in cell.Descendants ("a")) {
										string strNodeText = HttpUtility.HtmlDecode (node.InnerText);
										if(strNodeText.Contains("</img>") == false)
										{
											strTranslated = strNodeText;
										}
									}

								} else {
									strTranslated = HttpUtility.HtmlDecode (cell.InnerText);
								}
							} else {
								break;
							}
							hColumn++;
						}

						listTranslated.Add (new LangBaseClass {
							ShortName = strLabel,
							TranslatedAs = strTranslated
						});
					}
				}
			}
			catch(Exception e) {
				strData = "Error " + e.Message + ". " + strLabel + " " + strTranslated;
			}

			foreach (CSharpTree.TreeNode<LangRegionClass> lrc in lcc.MyTree) {

				if (lrc.IsRoot) {
					continue;
				}

				// lrc.Data.ShortName

				foreach (LangBaseClass lbc in listTranslated) {

					if (lrc.Data.ShortName == lbc.ShortName) {

						lrc.Data.TranslatedAs = lbc.TranslatedAs;

					}

				}

			}

		}

	}
}

