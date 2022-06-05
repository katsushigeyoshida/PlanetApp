using CoreLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PlanetApp
{
    /// <summary>
    /// WikiList.xaml の相互作用ロジック
    /// </summary>
    public partial class WikiList : Window
    {
        private double mWindowWidth = 0.0;                          //  ウィンドウの高さ
        private double mWindowHeight = 0.0;                         //  ウィンドウ幅
        private double mPrevWindowWidth = 0.0;                      //  変更前のウィンドウ幅

        private string[] mGetDataButtonLabel = { "詳細取得", "中断" };
        private enum PROGRESSMODE { NON, GETDETAIL, SEARCHFILE };
        private PROGRESSMODE mProgressMode = PROGRESSMODE.NON;

        private string mAppFolder;                              //  アプリケーションフォルダ
        private string mDataFolder = "WikiData";                //  WikiData データフォルダ
        private string mUrlListPath = "WikiDataUrlList.csv";    //  URLリストファイル
        private string mBaseUrl = "https://ja.wikipedia.org";   //  ベースURL(Wikipedia)
        private bool mGetInfoDataAbort = true;                  //  詳細データ取得
        private bool mInfoDataUpdate = true;                    //  詳細データ更新
        private int mDispSize = 3;                              //  表示データ数
        private int[] mDispStringSize = { -1, -1, 50, 100 };    //  リスト表示データ文字数

        private WikiUrlList mWikiUrlList = new WikiUrlList();
        private WikiDataList mWikiDataList = new WikiDataList();

        public MainWindow mMainWindow;

        private YLib ylib = new YLib();

        public WikiList()
        {
            InitializeComponent();

            WindowFormLoad();                       //  Windowの位置とサイズを復元
            mAppFolder = ylib.getAppFolderPath();   //  アプリフォルダ
            mDataFolder = Path.Combine(mAppFolder, mDataFolder);
            mUrlListPath = Path.Combine(mAppFolder, mUrlListPath);
            BtGetData.Content = mGetDataButtonLabel[0];

            //  一覧リストの登録
            setUrlList();
            //  検索方法の登録
            CbSeachForm.ItemsSource = mWikiDataList.mSearchFormTitle;
            CbSeachForm.SelectedIndex = 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WindowFormSave();       //  ウィンドの位置と大きさを保存
        }

        /// <summary>
        /// Windowの状態を前回の状態にする
        /// </summary>
        private void WindowFormLoad()
        {
            //  前回のWindowの位置とサイズを復元する(登録項目をPropeties.settingsに登録して使用する)
            Properties.Settings.Default.Reload();
            if (Properties.Settings.Default.WikiListWidth < 100 ||
                Properties.Settings.Default.WikiListHeight < 100 ||
                SystemParameters.WorkArea.Height < Properties.Settings.Default.WikiListHeight) {
                Properties.Settings.Default.WikiListWidth = mWindowWidth;
                Properties.Settings.Default.WikiListHeight = mWindowHeight;
            } else {
                Top = Properties.Settings.Default.WikiListTop;
                Left = Properties.Settings.Default.WikiListLeft;
                Width = Properties.Settings.Default.WikiListWidth;
                Height = Properties.Settings.Default.WikiListHeight;
            }
        }

        /// <summary>
        /// Window状態を保存する
        /// </summary>
        private void WindowFormSave()
        {
            //  Windowの位置とサイズを保存(登録項目をPropeties.settingsに登録して使用する)
            Properties.Settings.Default.WikiListTop = Top;
            Properties.Settings.Default.WikiListLeft = Left;
            Properties.Settings.Default.WikiListWidth = Width;
            Properties.Settings.Default.WikiListHeight = Height;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// [タイトル]コンボボックス URLの切替得
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbTitle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (0 <= CbTitle.SelectedIndex && mProgressMode == PROGRESSMODE.NON) {
                //  URLまたはファイルからWikiリストを取得
                LbUrlAddress.Content = mWikiUrlList.mUrlList[CbTitle.SelectedIndex][1];
                getWikiDataList(mWikiUrlList.mUrlList[CbTitle.SelectedIndex][1]);
            }
        }

        /// <summary>
        /// [一覧データ抽出形式]コンボボックス
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbSeachForm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// [URLアドレス]コンテキストメニュー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LbUrlContextMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)e.Source;
            if (menuItem.Name.CompareTo("LbUrlCopyMenu") == 0) {
                //  URLのコピー
                Clipboard.SetText(LbUrlAddress.Content.ToString());
            } else if (menuItem.Name.CompareTo("LbUrlOpenMenu") == 0) {
                //  URLを開く
                //System.Diagnostics.Process.Start(LbUrlAddress.Content.ToString());
                ylib.openUrl(LbUrlAddress.Content.ToString());
            } else if (menuItem.Name.CompareTo("LbUrlAddMenu") == 0) {
                //  URLの追加
                if (mWikiUrlList.addUrlList())
                    setUrlList();
            } else if (menuItem.Name.CompareTo("LbUrlRemoveMenu") == 0) {
                //  URLの削除
                if (0 <= CbTitle.SelectedIndex) {
                    var result = MessageBox.Show("[" + mWikiUrlList.mUrlList[CbTitle.SelectedIndex][0] + "] を削除します", "削除確認", MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK) {
                        mWikiUrlList.mUrlList.RemoveAt(CbTitle.SelectedIndex);
                        setUrlList();
                    }
                }
            }
        }

        /// <summary>
        /// [データリスト]コンテキストメニュー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgWikiListMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)e.Source;
            if (menuItem.Name.CompareTo("DgDispMenu") == 0) {
                //  詳細表示
                if (0 <= DgDataList.SelectedIndex) {
                    string title = ((string[])DgDataList.Items[DgDataList.SelectedIndex])[0];
                    WikiData wikiData = mWikiDataList.getTitleData(title);
                    string buf = wikiData.mData;
                    if (0 < wikiData.mListTitle.Length)
                        buf += "\n親リストタイトル: " + wikiData.mListTitle;
                    if (0 < wikiData.mListUrl.Length)
                        buf += "\n親リストURL: " + wikiData.mListUrl;
                    if (0 < wikiData.mSearchForm.Length)
                        buf += "\n抽出方法: " + wikiData.mSearchForm;
                    messageBox(buf, "基本情報表示");
                }
            } else if (menuItem.Name.CompareTo("DgCopyMenu") == 0) {
                //  選択データのコピー
                if (0 < DgDataList.SelectedItems.Count) {
                    string buffer = string.Join(",", mWikiDataList.mTitle);
                    foreach (string[] data in DgDataList.SelectedItems) {
                        WikiData wikidata = mWikiDataList.mDataList.Find(p => p.mTitle.CompareTo(data[0]) == 0);
                        buffer += "\n";
                        buffer += wikidata.ToString();
                    }
                    Clipboard.SetText(buffer);
                }
            } else if (menuItem.Name.CompareTo("DgOpenMenu") == 0) {
                //  選択アイテムのURLを開く
                if (0 <= DgDataList.SelectedIndex) {
                    string title = ((string[])DgDataList.Items[DgDataList.SelectedIndex])[0];
                    string url = mWikiDataList.getTitleData(title).mUrl;
                    if (url.IndexOf("http") != 0)
                        url = mBaseUrl + url;
                    ylib.openUrl(url);
                }
            } else if (menuItem.Name.CompareTo("DgRemoveMenu") == 0) {
                //  データ削除
                if (0 < DgDataList.SelectedItems.Count) {
                    foreach (string[] data in DgDataList.SelectedItems) {
                        WikiData wikidata = mWikiDataList.mDataList.Find(p => p.mTitle.CompareTo(data[0]) == 0);
                        mWikiDataList.mDataList.Remove(wikidata);
                    }
                    curWikiListSave();
                    setDispWikiData();
                }
            } else if (menuItem.Name.CompareTo("DgRemoveDupulicateUrl") == 0) {
                //  URL重複行削除
                mWikiDataList.mDataList = mWikiDataList.removeDulicateUrl();
                curWikiListSave();
                setDispWikiData();
            } else if (menuItem.Name.CompareTo("DgUndispRemoveMenu") == 0) {
                //  非表示データ削除
                List<string> dispTitle = new List<string>();
                foreach (string[] wikidata in DgDataList.SelectedItems) {
                    dispTitle.Add(wikidata[0]);
                }
                for (int i = mWikiDataList.mDataList.Count - 1; 0 <= i; i--) {
                    if (0 > dispTitle.IndexOf(mWikiDataList.mDataList[i].mTitle))
                        mWikiDataList.mDataList.RemoveAt(i);
                }
                curWikiListSave();
                setDispWikiData();
            } else if (menuItem.Name.CompareTo("DgDispRemoveMenu") == 0) {
                //  表示データ削除
                foreach (string[] data in DgDataList.Items) {
                    WikiData wikidata = mWikiDataList.mDataList.Find(p => p.mTitle.CompareTo(data[0]) == 0);
                    mWikiDataList.mDataList.Remove(wikidata);
                }
                curWikiListSave();
                setDispWikiData();
            }
        }
        
        /// <summary>
        /// [一覧更新]ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtUpdateData_Click(object sender, RoutedEventArgs e)
        {
            if (0 <= CbTitle.SelectedIndex) {
                getWikiDataList(mWikiUrlList.mUrlList[CbTitle.SelectedIndex][1], true);
            }
        }

        /// <summary>
        /// [詳細取得]ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtGetData_Click(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)e.Source;
            if (bt.Content.ToString().CompareTo(mGetDataButtonLabel[0]) == 0) {
                CbTitle.IsEnabled = false;
                mGetInfoDataAbort = false;
                mInfoDataUpdate = true;
                mDispSize = 4;
                getInfoData(mBaseUrl);
                bt.Content = mGetDataButtonLabel[1];
            } else if (bt.Content.ToString().CompareTo(mGetDataButtonLabel[1]) == 0) {
                //  登録処理中断のフラグを設定
                CbTitle.IsEnabled = true;
                mGetInfoDataAbort = true;
                bt.Content = mGetDataButtonLabel[0];
            }
        }

        /// <summary>
        /// [詳細表示]ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtInfoData_Click(object sender, RoutedEventArgs e)
        {
            if (mDispSize == 4) {
                mDispSize = 3;
            } else {
                mDispSize = 4;
            }
            setDispWikiData();
        }

        /// <summary>
        /// [検索URL]ダフルクリックで一覧リストの Webページを開く
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LbUrlAddress_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (LbUrlAddress.Content.ToString().Length != 0) {
                ylib.openUrl(LbUrlAddress.Content.ToString());
            }
        }

        /// <summary>
        /// [フィルタ]ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtFilter_Click(object sender, RoutedEventArgs e)
        {
            setDispWikiData(TbSearch.Text);
        }

        /// <summary>
        /// [前検索]ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtPrevSearch_Click(object sender, RoutedEventArgs e)
        {
            if (0 < TbSearch.Text.Length) {
                int selectIndex = DgDataList.SelectedIndex;
                selectIndex = prevSearch(TbSearch.Text, selectIndex - 1);
                if (0 <= selectIndex) {
                    DgDataList.SelectedIndex = selectIndex;
                    DgDataList.ScrollIntoView(DgDataList.SelectedItem);
                }
            }
        }

        /// <summary>
        /// [次検索]ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtNextSearch_Click(object sender, RoutedEventArgs e)
        {
            if (0 < TbSearch.Text.Length) {
                int selectIndex = DgDataList.SelectedIndex;
                selectIndex = nextSearch(TbSearch.Text, selectIndex + 1);
                if (0 <= selectIndex) {
                    DgDataList.SelectedIndex = selectIndex;
                    DgDataList.ScrollIntoView(DgDataList.SelectedItem);
                }
            }
        }

        /// <summary>
        /// [検索]ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtSearch_Click(object sender, RoutedEventArgs e)
        {
            if (0 < TbSearch.Text.Length) {
                mGetInfoDataAbort = false;      //  中断フラグ
                string fileName = CbTitle.SelectedIndex <= 0 ? "" : mWikiUrlList.mUrlList[CbTitle.SelectedIndex][0];
                getSearchFileWikiData(TbSearch.Text, mDataFolder, fileName);
            }
        }

        /// <summary>
        /// [データリスト]ダブルクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgWikiList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (0 <= DgDataList.SelectedIndex) {
                string title = ((string[])DgDataList.Items[DgDataList.SelectedIndex])[0];
                string url = mWikiDataList.getTitleData(title).mUrl;
                if (url.IndexOf("http") != 0)
                    url = mBaseUrl + url;
                ylib.openUrl(url);
            }
        }

        /// <summary>
        /// [データリスト]選択行変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgWikiList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// [プログレスバー]詳細データの取得完了処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PbGetInfoData_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (PbGetInfoData.Value == PbGetInfoData.Maximum || mGetInfoDataAbort) {
                progressTerminate();
            }
        }

        /// <summary>
        /// [ヘルプ]ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtHelp_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// URLのデータリストのタイトルをCbTitleのコンボボックスに設定
        /// </summary>
        private void setUrlList()
        {
            CbTitle.Items.Clear();
            foreach (string[] data in mWikiUrlList.mUrlList)
                CbTitle.Items.Add(data[0]);
            CbTitle.SelectedIndex = 0;
            if (0 <= CbTitle.SelectedIndex)
                LbUrlAddress.Content = mWikiUrlList.mUrlList[CbTitle.SelectedIndex][1];
            else
                LbUrlAddress.Content = "";
        }

        /// <summary>
        /// 一覧Webページのリストデータを取得して表示
        /// </summary>
        /// <param name="url">一覧のURL</param>
        /// <param name="update">強制更新</param>
        private void getWikiDataList(string url, bool update = false)
        {
            if (url != null && url.IndexOf("http") == 0) {
                string title = url.Substring(url.LastIndexOf("/") + 1).Replace(':', '_');
                string filePath = Path.Combine(mDataFolder, title + ".csv");
                if (!File.Exists(filePath) || update) {
                    string htmlPath = Path.Combine(mDataFolder, title + ".html");
                    if (!File.Exists(htmlPath) || update) {
                        //  Webからデータのダウンロード
                        if (ylib.makeDir(htmlPath)) {
                            ylib.webFileDownload(url, htmlPath);
                        } else {
                            return;
                        }
                    }
                    string html = ylib.loadTextFile(htmlPath);
                    setListSearchForm();
                    mWikiDataList.getWikiDataList(html);
                    mWikiDataList.addData(title, url, mWikiDataList.mSearchForm);
                    mWikiDataList.saveData(filePath);
                } else {
                    //  ファイルからデータを取得
                    mWikiDataList.loadData(filePath);
                }
            } else {
                //  空データ
                mWikiDataList.mDataList.Clear();
            }
            setDispWikiData();
        }

        /// <summary>
        /// Webからの詳細データの取得進捗の表示(非同期処理)
        /// </summary>
        private void getInfoData(string baseUrl)
        {
            LbSearchForm.Content = "";
            PbGetInfoData.Maximum = mWikiDataList.mDataList.Count - 1;
            PbGetInfoData.Minimum = 0;
            PbGetInfoData.Value = 0;
            mProgressMode = PROGRESSMODE.GETDETAIL;
            //  非同期処理
            Task.Run(() => {
                for (int i = 0; i < mWikiDataList.mDataList.Count; i++) {
                    if (mGetInfoDataAbort)                          //  中断フラグ
                        break;
                    mWikiDataList.mDataList[i].getTagSetData(mBaseUrl);     //  基本情報の取得
                    Application.Current.Dispatcher.Invoke(() => {
                        PbGetInfoData.Value = i;
                        LbGetDataProgress.Content = "進捗 " + (i + 1) + " / " + mWikiDataList.mDataList.Count;
                    });
                }
                Application.Current.Dispatcher.Invoke(() => {
                    PbGetInfoData.Value = PbGetInfoData.Maximum;
                });
            });
        }

        /// <summary>
        /// 全ファイルの中から検索する
        /// </summary>
        /// <param name="searchText">検索文字列</param>
        /// <param name="dataFoleder">検索ファイルのフォルダ</param>
        /// <param name="fileName">検索ファイル名</param>
        public void getSearchFileWikiData(string searchText, string dataFoleder, string fileName = "")
        {
            //  対象ファイルの検索
            LbSearchForm.Content = "";
            string[] fileList = ylib.getFiles(dataFoleder + "\\" + (fileName.Length == 0 ? "*" : fileName) + ".csv");
            if (fileList != null) {
                mWikiDataList.mDataList.Clear();

                //  一覧選択を無効にする
                CbTitle.SelectedIndex = -1;
                LbUrlAddress.Content = "";

                //  プログレスバー初期化
                PbGetInfoData.Maximum = fileList.Length;
                PbGetInfoData.Minimum = 0;
                PbGetInfoData.Value = 0;
                mProgressMode = PROGRESSMODE.SEARCHFILE;

                //  非同期処理
                Task.Run(() => {
                    int count = 1;
                    foreach (string path in fileList) {
                        if (mGetInfoDataAbort)                          //  中断フラグ
                            break;
                        //  ファイルごとのデータ検索
                        mWikiDataList.getSerchWikiDataFile(searchText, path);
                        Application.Current.Dispatcher.Invoke(() => {
                            PbGetInfoData.Value = count;
                            LbGetDataProgress.Content = "検出数 " + mWikiDataList.mDataList.Count;
                        });
                        count++;
                    }
                    //  検索結果の処理
                    if (0 < mWikiDataList.mDataList.Count) {
                    }
                    Application.Current.Dispatcher.Invoke(() => {
                        PbGetInfoData.Value = PbGetInfoData.Maximum;
                    });
                });
            }
        }


        /// <summary>
        /// Webからのデータ取得の終了処理
        /// </summary>
        private void getInfoDataTerminate()
        {
            BtGetData.Content = mGetDataButtonLabel[0];
            CbTitle.IsEnabled = true;
            if (mInfoDataUpdate) {
                //  ヘッダの更新
                mInfoDataUpdate = false;
            }
            //  データの保存と表示
            curWikiListSave();
            setDispWikiData();
        }

        /// <summary>
        /// ファイル内検索終了処理
        /// </summary>
        private void getSearchFileTextTermnate()
        {
            setDispWikiData();
        }

        /// <summary>
        /// 詳細データ取得または検索処理後の処理
        /// </summary>
        private void progressTerminate()
        {
            //  プログレスバー,メッセージ、ボタン名などの初期化
            if (mProgressMode != PROGRESSMODE.NON) {
                PbGetInfoData.Value = 0;
                LbGetDataProgress.Content = "完了";
                mGetInfoDataAbort = false;
            }
            if (mProgressMode == PROGRESSMODE.GETDETAIL) {
                getInfoDataTerminate();
            } else if (mProgressMode == PROGRESSMODE.SEARCHFILE) {
                getSearchFileTextTermnate();
            }
            PbGetInfoData.Value = 0;
            mProgressMode = PROGRESSMODE.NON;
        }

        /// <summary>
        /// 現在のデータを保存する
        /// </summary>
        private void curWikiListSave()
        {
            string url = mWikiUrlList.mUrlList[CbTitle.SelectedIndex][1];
            string title = url.Substring(url.LastIndexOf("/") + 1).Replace(':', '_');
            string filePath = Path.Combine(mDataFolder, title + ".csv");
            mWikiDataList.saveData(filePath);
        }

        /// <summary>
        /// 現在のデータを表示する
        /// </summary>
        /// <param name="filter">表示フィルタ(ワイルドカード)</param>
        private void setDispWikiData(string filter = "")
        {
            if (0 < mWikiDataList.mDataList.Count) {
                setTitle(mWikiDataList.mTitle);
                setData(mWikiDataList.getArrayList(), filter);
                LbGetDataProgress.Content = "データ数 " + DgDataList.Items.Count + " / " + mWikiDataList.mDataList.Count;
            }
        }

        /// <summary>
        /// データのタイトルをDataGridに設定する
        /// </summary>
        /// <param name="title">タイトル配列</param>
        private void setTitle(string[] title)
        {
            DgDataList.Columns.Clear();
            int dataSize = Math.Min(title.Length, mDispSize);
            for (int i = 0; i < dataSize; i++) {
                var column = new DataGridTextColumn();
                column.Header = title[i];
                column.Binding = new Binding($"[{i}]");
                DgDataList.Columns.Add(column);
            }
        }

        /// <summary>
        /// データをDataGridに設定する
        /// </summary>
        /// <param name="dataList">データリスト</param>
        private void setData(List<string[]> dataList, string filter = "")
        {
            int dataSize = Math.Min(dataList[0].Length, mDispSize);
            DgDataList.Items.Clear();
            for (int i = 0; i < dataList.Count; i++) {
                if (0 == filter.Length || 0 <= dataList[i][0].IndexOf(filter) || ylib.wcMatch(dataList[i][0], filter)) {
                    string[] buf = new string[dataSize];
                    for (int j = 0; j < dataSize; j++) {
                        int n = 0;
                        if (j == 3 && 0 <= (n = dataList[i][j].IndexOf("\n"))) {
                            buf[j] = dataList[i][j].Substring(n + 1);
                        } else {
                            buf[j] = dataList[i][j];
                        }
                        buf[j] = buf[j].Replace("\n", "_");
                        buf[j] = buf[j].Replace("\r", "");
                        if (j < mDispStringSize.Length &&  0 < mDispStringSize[j] ) {
                            int stringSize = Math.Min(buf[j].Length, mDispStringSize[j]);
                            buf[j] = buf[j].Substring(0, stringSize);
                        }
                    }
                    DgDataList.Items.Add(buf);
                }
            }
        }

        /// <summary>
        /// Webソースから一覧リストを抽出る方法を設定する
        /// </summary>
        private void setListSearchForm()
        {
            mWikiDataList.mSearchForm = (WikiDataList.SEARCHFORM)Enum.ToObject(typeof(WikiDataList.SEARCHFORM), CbSeachForm.SelectedIndex);
        }

        /// <summary>
        /// タイトルの後方検索
        /// </summary>
        /// <param name="word">検索ワード</param>
        /// <param name="n">検索開始行</param>
        /// <returns>検索位置</returns>
        private int prevSearch(string word, int n)
        {
            n = Math.Min(n, DgDataList.Items.Count - 1);
            for (int i = n; 0 <= i ; i--) {
                string[] data = (string[])DgDataList.Items.GetItemAt(i);
                if (0 <= data[0].IndexOf(word) || ylib.wcMatch(data[0], word))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// タイトルの前方検索
        /// </summary>
        /// <param name="word">検索ワード</param>
        /// <param name="n">検索開始行</param>
        /// <returns>検索位置</returns>
        private int nextSearch(string word, int n)
        {
            n = Math.Max(0, n);
            for (int i = n; i < DgDataList.Items.Count; i++) {
                string[] data = (string[])DgDataList.Items.GetItemAt(i);
                if (0 <= data[0].IndexOf(word) || ylib.wcMatch(data[0], word))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// メッセージ表示
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="title"></param>
        private void messageBox(string buf, string title)
        {
            InputBox dlg = new InputBox();
            dlg.Title = title;
            dlg.mWindowSizeOutSet = true;
            dlg.mWindowWidth = 500.0;
            dlg.mWindowHeight = 400.0;
            dlg.mMultiLine = true;
            dlg.mReadOnly = true;
            dlg.mEditText = buf;
            dlg.ShowDialog();
        }
    }
}
