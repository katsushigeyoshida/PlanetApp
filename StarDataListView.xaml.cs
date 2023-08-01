using CoreLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PlanetApp
{
    /// <summary>
    /// StarDataList.xaml の相互作用ロジック
    ///
    /// 恒星データを表形式で表示する
    /// </summary>
    public partial class StarDataListView : Window
    {
        private double mWindowWidth;                            //  ウィンドウの高さ
        private double mWindowHeight;                           //  ウィンドウ幅
        private double mPrevWindowWidth;                        //  変更前のウィンドウ幅
        private WindowState mWindowState = WindowState.Normal;  //  ウィンドウの状態(最大化/最小化)

        public string mAppFolder;                               //  アプリケーションのフォルダ
        public string mDataFolder;                              //  データファイルのフォルダ
        public string[] mListTitle;                             //  恒星データのタイトル
        public List<string[]> mStarDataList;                    //  恒星データリスト
        public string mStatusInfo = "";                         //  リスト情報

        private bool mOrginalDisp = false;
        private string[] mFileList;                             //  データファイルリスト
        public string[] mOperation = { 
            "データ表示", "オリジナル表示", "データ結合", "CSV保存"
        };

        private StarData mStarData;

        public MainWindow mMainWindow;

        private YLib ylib = new YLib();

        public StarDataListView()
        {
            InitializeComponent();

            WindowFormLoad();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mStarData = new StarData(mAppFolder, mDataFolder);
            //  データファイルリストの登録
            mFileList = ylib.getFiles(mDataFolder + "\\*.csv");
            CbDataFIleList.Items.Clear();
            for (int i = 0; i < mFileList.Length; i++) {
                CbDataFIleList.Items.Add(Path.GetFileName(mFileList[i]));
            }
            //  メニューの登録
            CbMenu.ItemsSource = mOperation;
            CbMenu.SelectedIndex = 0;

            //  データを表形式で表示
            dispData(mStarDataList);
        }

        private void Window_LayoutUpdated(object sender, EventArgs e)
        {
            if (WindowState != mWindowState &&
                 WindowState == WindowState.Maximized) {
                //  ウィンドウの最大化時
                mWindowWidth = SystemParameters.WorkArea.Width;
                mWindowHeight = SystemParameters.WorkArea.Height;
            } else if (this.WindowState != mWindowState ||
                mWindowWidth != Width ||
                mWindowHeight != Height) {
                //  ウィンドウサイズが変わった時
                mWindowWidth = Width;
                mWindowHeight = Height;
            } else {
                //  ウィンドウサイズが変わらない時は何もしない
                mWindowState = WindowState;
                return;
            }

            //  ウィンドウの大きさに合わせてコントロールの幅を変更する
            double dx = mWindowWidth - mPrevWindowWidth;
            //titleList.Width += dx;
            //functionList.Width += dx;
            mPrevWindowWidth = mWindowWidth;

            mWindowState = WindowState;
            //DrawGraph();        //  グラフ表示
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WindowFormSave();
        }


        /// <summary>
        /// Windowの状態を前回の状態にする
        /// </summary>
        private void WindowFormLoad()
        {
            //  前回のWindowの位置とサイズを復元する(登録項目をPropeties.settingsに登録して使用する)
            Properties.Settings.Default.Reload();
            if (Properties.Settings.Default.StarDataListViewWidth < 100 || Properties.Settings.Default.StarDataListViewHeight < 100 ||
                SystemParameters.WorkArea.Height < Properties.Settings.Default.StarDataListViewHeight) {
                Properties.Settings.Default.StarDataListViewWidth = mWindowWidth;
                Properties.Settings.Default.StarDataListViewHeight = mWindowHeight;
            } else {
                Top = Properties.Settings.Default.StarDataListViewTop;
                Left = Properties.Settings.Default.StarDataListViewLeft;
                Width = Properties.Settings.Default.StarDataListViewWidth;
                Height = Properties.Settings.Default.StarDataListViewHeight;
                double dy = Height - mWindowHeight;
            }
        }

        /// <summary>
        /// Window状態を保存する
        /// </summary>
        private void WindowFormSave()
        {
            //  Windowの位置とサイズを保存(登録項目をPropeties.settingsに登録して使用する)
            Properties.Settings.Default.StarDataListViewTop = Top;
            Properties.Settings.Default.StarDataListViewLeft = Left;
            Properties.Settings.Default.StarDataListViewWidth = Width;
            Properties.Settings.Default.StarDataListViewHeight = Height;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// [操作メニュー]選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (0 <= CbMenu.SelectedIndex) {
                switch (CbMenu.SelectedIndex) {

                    case 0:     //  データ表示
                        mOrginalDisp = false;
                        if (0 <= CbDataFIleList.SelectedIndex)
                            getFiledata(mFileList[CbDataFIleList.SelectedIndex], mOrginalDisp);
                        break;
                    case 1:     //  オリジナル表示
                        mOrginalDisp = true;
                        if (0 <= CbDataFIleList.SelectedIndex)
                            getFiledata(mFileList[CbDataFIleList.SelectedIndex], mOrginalDisp);
                        break;
                    case 2:         //  データ結合
                        fileMerge();
                        break;
                    case 3:         //  CSV保存
                        saveData();
                        break;
                        //case 2:         //  標準データに変換
                        //    convStandard();
                        //    break;
                        //case 2:         //  表示データに設定
                        //    setStandardFile();
                        //    break;
                        //case 3:         //  元データ表示
                        //    fileSelectLoad(true);
                        //    break;
                }
            }
        }

        /// <summary>
        /// [ファイル]選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbDataFIleList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (0 <= CbDataFIleList.SelectedIndex) {
                getFiledata(mFileList[CbDataFIleList.SelectedIndex], mOrginalDisp);
            }
        }

        /// <summary>
        /// [リスト]ダブルクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgStarData_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (0 <= DgStarData.SelectedIndex) {
                if (mMainWindow != null) {
                    //  MainWindowの恒星にマーク表示をおこなう
                    mMainWindow.mMarkStarData = DgStarData.SelectedIndex;
                    mMainWindow.drawPlanet(false, new PointD());
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
            if (0 < TbSearchText.Text.Length) {
                int selectIndex = DgStarData.SelectedIndex;
                selectIndex = nextSearch(TbSearchText.Text, selectIndex + 1);
                if (0 <= selectIndex) {
                    DgStarData.SelectedIndex = selectIndex;
                    DgStarData.ScrollIntoView(DgStarData.SelectedItem);
                }
            }
        }

        /// <summary>
        /// データファイル読込表示
        /// </summary>
        /// <param name="original">赤経、赤緯まとめ</param>
        private void fileSelectLoad(bool original = false)
        {
            List<string[]> filters = new List<string[]>() {
                    new string[] { "CSVファイル", "*.csv;*.csv" },
                    new string[] { "すべてのファイル", "*.*"}
                };
            string filePath = ylib.fileOpenSelectDlg("データ読込", mDataFolder, filters);
            if (0 < filePath.Length)
                getFiledata(filePath, original);
        }

        /// <summary>
        /// ファイルマージ
        /// </summary>
        private void fileMerge()
        {
            SelectMenu selectMenu = new SelectMenu();
            selectMenu.mMenuList = mStarData.mOrgStarStrData[0].InsertTop("共有列を選択");
            if (selectMenu.ShowDialog() == true) {
                List<string[]> filters = new List<string[]>() {
                    new string[] { "CSVファイル", "*.csv;*.csv" },
                    new string[] { "すべてのファイル", "*.*"}
                };
                string filePath = ylib.fileOpenSelectDlg("データ読込", mDataFolder, filters);
                if (0 < filePath.Length) {
                    if (fileMerge(filePath, selectMenu.mSelectItem))
                        Title += "  " + Path.GetFileNameWithoutExtension(filePath);
                }
            }
        }

        /// <summary>
        /// ファイルマージ
        /// </summary>
        /// <param name="path">マージファイル</param>
        /// <param name="mergeTitle">マージ・キータイトル</param>
        /// <returns>可否</returns>
        private bool fileMerge(string path, string mergeTitle)
        {
            StarData starData = new StarData(mAppFolder, mDataFolder);
            if (starData.loadStarData(path)) {
                string destTitle = mergeTitle;
                if (!starData.mOrgStarStrData[0].Exists(mergeTitle)) {
                    SelectMenu selectMenu = new SelectMenu();
                    selectMenu.mMenuList = starData.mOrgStarStrData[0].InsertTop("共有列を選択");
                    if (selectMenu.ShowDialog() == false)
                        return false; ;
                    destTitle = selectMenu.mSelectItem;
                }
                mStarData.fileMerge(starData.mOrgStarStrData, starData.mOrgStarStrData[0], mergeTitle, destTitle);
                dispData(mStarData.mOrgStarStrData);
                return true;
            }
            return false;
        }

        /// <summary>
        /// データを標準形式に変換して表示
        /// </summary>
        //private void convStandard()
        //{
        //    mStarData.convStanderdData();
        //    dispData(mStarData.mStarStrData, mStarData.mStarStrDataTitle);
        //}

        /// <summary>
        /// 表示データに設定
        /// </summary>
        private void setStandardFile()
        {
            if (mStarData.mStarStrData != null && 0 < mStarData.mStarStrData.Count) {
                InputBox dlg = new InputBox();
                dlg.Owner = mMainWindow;
                dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                dlg.mEditText = mStarData.mStarTableTitle;
                if (dlg.ShowDialog() == true) {
                    mStarData.saveStandardData(dlg.mEditText);
                }
            }
        }

        /// <summary>
        /// 表示データのCSV保存
        /// </summary>
        private void saveData()
        {
            if (0 < mStarData.mOrgStarStrData.Count) {
                List<string[]> filters = new List<string[]>() {
                    new string[] { "CSVファイル", "*.csv;*.csv" },
                    new string[] { "すべてのファイル", "*.*"}
                };
                string filePath = ylib.fileSaveSelectDlg("データ保存", mDataFolder, filters);
                ylib.mEncordingType = 1;            //  SJIS
                if (0 < filePath.Length) {
                    ylib.saveCsvData(filePath, mStarData.mOrgStarStrData);
                }
            }
        }

        /// <summary>
        /// 指定ファイルのデータを読み込んで表示
        /// </summary>
        /// <param name="path">データファイルパス</param>
        /// <param name="original">オリジナル表示</param>
        private void getFiledata(string path, bool original = false)
        {
            if (original) {
                mStarData.loadOrgStarData(path);
            } else {
                mStarData.loadStarData(path);
            }
            dispData(mStarData.mOrgStarStrData);
            Title = Path.GetFileNameWithoutExtension(path);
        }

        /// <summary>
        /// データを表に設定
        /// </summary>
        /// <param name="starData">タイトルデータ</param>
        /// <param name="dataTitle">データリスト</param>
        private void dispData(List<string[]> starData)
        {
            //  データを表形式で表示
            setTitle(starData[0]);
            setData(starData);
            //  データの情報表示
            TbStatus.Text = (starData.Count - 1).ToString("#,##0") + " 行  "
                    + starData[0].Length.ToString("#,##0") + " 列";
        }

        /// <summary>
        /// データのタイトルをDataGridに設定する
        /// </summary>
        /// <param name="title">タイトル配列</param>
        private void setTitle(string[] title)
        {
            DgStarData.Columns.Clear();
            for (int i = 0; i < title.Length; i++) {
                var column = new DataGridTextColumn();
                column.Header = title[i];
                column.Binding = new Binding($"[{i}]");
                DgStarData.Columns.Add(column);
            }
        }

        /// <summary>
        /// データをDataGridに設定する
        /// </summary>
        /// <param name="dataList">データリスト</param>
        private void setData(List<string[]> dataList)
        {
            DgStarData.Items.Clear();
            for (int i = 1; i < dataList.Count; i++) {
                DgStarData.Items.Add(dataList[i]);
            }
        }

        /// <summary>
        /// 時検索(ワイルドカード使用可)
        /// 全カラムから検索する
        /// </summary>
        /// <param name="word">検索ワード</param>
        /// <param name="n">検索開始行</param>
        /// <returns>検索行</returns>
        private int nextSearch(string word, int n)
        {
            n = Math.Max(0, n);
            for (int i = n; i < DgStarData.Items.Count; i++) {
                string[] data = (string[])DgStarData.Items.GetItemAt(i);
                foreach (string text in data) {
                    if (0 <= text.IndexOf(word) || ylib.wcMatch(text, word))
                        return i;
                }
            }
            return -1;
        }
    }
}
