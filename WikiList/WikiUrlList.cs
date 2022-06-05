using CoreLib;
using System;
using System.Collections.Generic;

namespace PlanetApp
{
    internal class WikiUrlList
    {
        public List<string[]> mUrlList = new List<string[]>() {
            new string[]{ "Wikipediaの一覧リスト",   ""},
            new string[]{ "明るい恒星の一覧", "https://ja.wikipedia.org/wiki/明るい恒星の一覧" },
            new string[]{ "太陽系の天体の一覧", "https://ja.wikipedia.org/wiki/太陽系の天体の一覧" },
            new string[]{ "星座の一覧", "https://ja.wikipedia.org/wiki/星座の一覧" },
            new string[]{ "メシエ天体の一覧", "https://ja.wikipedia.org/wiki/メシエ天体の一覧" },
            new string[]{ "Category:太陽系の惑星", "https://ja.wikipedia.org/wiki/Category:太陽系の惑星" },
            new string[]{ "大きさ順の太陽系天体の一覧", "https://ja.wikipedia.org/wiki/大きさ順の太陽系天体の一覧" }
        };
        private string[] mUrlListFormat = new string[] { "タイトル", "URLアドレス" };

        private YLib ylib = new YLib();

        /// <summary>
        /// URLの一覧リストを読み込む
        /// </summary>
        /// <param name="filePath"></param>
        public bool loadUrlList(string filePath)
        {
            List<string[]> urlList = ylib.loadCsvData(filePath, mUrlListFormat);
            if (urlList != null) {
                foreach (string[] urlData in urlList) {
                    if (!existUrl(urlData))
                        mUrlList.Add(urlData);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// URLリストに登録されているかの確認
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private bool existUrl(string[] url)
        {
            foreach (string[] data in mUrlList)
                if (url[0].CompareTo(data[0]) == 0)
                    return true;
            return false;
        }

        /// <summary>
        /// URLの一覧リストを保存する
        /// </summary>
        /// <param name="filePath"></param>
        public void saveUrlList(string filePath)
        {
            ylib.saveCsvData(filePath, mUrlListFormat, mUrlList);
        }

        /// <summary>
        /// ダイヤログを表示してURLをリストに追加する
        /// </summary>
        public bool addUrlList()
        {
            InputBox2 dlg = new InputBox2();
            dlg.Title = "一覧リストWebページ設定";
            dlg.mTitle1 = "リストタイトル";
            dlg.mTitle2 = "ＵＲＬアドレス";
            var result = dlg.ShowDialog();
            if (result == true) {
                string[] data = new string[2];
                data[0] = dlg.mEditText1;
                data[1] = Uri.UnescapeDataString(dlg.mEditText2);
                if (data[0].Length <= 0)
                    data[0] = data[1].Substring(data[1].LastIndexOf('/') + 1);
                data[0] = data[0].Replace(':', '_');
                if (!existUrl(data))
                    mUrlList.Add(data);
                return true;
            }
            return false;
        }
    }
}
