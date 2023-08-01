using CoreLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PlanetApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double mWindowWidth;                            //  ウィンドウの高さ
        private double mWindowHeight;                           //  ウィンドウ幅
        private double mPrevWindowWidth;                        //  変更前のウィンドウ幅
        private WindowState mWindowState = WindowState.Normal;  //  ウィンドウの状態(最大化/最小化)

        private string mAppFolder;                              //  アプリケーションのフォルダ
        private string mDataFolder = "StarData";                //  データフォルダ
        private string[] mDataFileList;                         //  恒星データファイルリスト

        private string mStarDataOrgPath = "hip_100.csv";        //  ヒッパルコス恒星データ
        private string mStarDataPath = "hip_100.csv";           //  表示恒星データ
        private string mHelpFile = "PlanetAppManual.pdf";       //  PDFのヘルプファイル

        private List<string[]> mHorizonStarData;                //  表示恒星の地表座標データ
        private string[] mHorizonStarDataTitle;                 //  表示恒星の地表座標データタイトル
        private string mStarDataInfo;                           //  表示データ情報(日時,恒星時,地方恒星時,観測地緯度)

        private StarData mStarData;                             //  恒星データ
        private ConstellationData mConstellationData;           //  星座データ
        private StarInfoData mStarInfoData;                     //  恒星情報データ
        private NebulaData mNebulaData;                         //  星雲、銀河など
        private Milkyway mMilkywayData;                         //  天の川データ

        private DateTime mDateTime;                             //  表示日時
        private double mLocalLatitude = 35.6581;                //  観測点の緯度(東京)
        private double mLocalLongitude = 139.7414;              //  観測点の経度(東京)
        private string[] mAzimuthName = { "南", "西", "北", "東" };
        private double mDirection = 12;                         //  表示方向(時)

        private double mPlanetWindowSize = 110.0;               //  表示領域のWindowサイズ(中心を原点としてのサイズ))
        private double mCelestialRadius = 100.0;                //  天球表示の半径
        private PointD mLeftPressPoint = new PointD();          //  マウス左ボタン位置

        private double mStarNemeTextSize = 15.0;                //  恒星名の文字サイズ
        private double mStarInfoTextSize = 12.0;                //  恒星情報の文字サイズ
        private double mStarRadiusRate = 3.0;                   //  恒星の表示サイズ比率
        private double mScaleTextSize = 18.0;                   //  目盛りの文字サイズ
        private double mScaleLargeTextSize = 25.0;              //  目盛りの文字サイズ大
        private int mMilkywayDispDinsity = 50;                  //  天の川表示濃度
        private Brush mBaseBackColor = Brushes.White;           //  全体の背景色
        private Brush mBackGroundColor = Brushes.Cyan;          //  背景色
        private Brush mBackGroundBorderColor = Brushes.Black;   //  背景の境界色
        private Brush mScaleTextColor = Brushes.Black;          //  目盛りの文字色
        private Brush mScaleLargeTextColor = Brushes.Black;     //  目盛りの文字色
        private Brush mScaleOutlineColor = Brushes.Black;       //  目盛りの枠線の色
        private Brush mHorizontalTextColor = Brushes.Yellow;    //  地平線の境界色(天球表示)
        private Brush mAuxLineColor = Brushes.LightGray;        //  補助線の色
        private Brush mHorizontalLineColor = Brushes.Red;       //  地平線線の色
        private Brush mStarFillColor = Brushes.White;           //  恒星の中の色
        private Brush mStarBorderColor = Brushes.Black;         //  恒星の境界色
        private Brush mStarNameColor = Brushes.Black;           //  恒星名の色
        private Brush mConstellaNameColor = Brushes.Blue;       //  星座名の色
        private Brush mNebulaNamColor = Brushes.Gray;           //  星雲・銀河などの色
        private Brush mNeburaBorderColor = Brushes.Blue;        //  星雲・銀河などの境界色
        private Brush mConstellaLineColor = Brushes.LightSteelBlue;       //  星座線の色
        private Brush mMarckColor = Brushes.Red;                //  恒星マークの色

        enum DISPPLANETTYPE { CELESTIAL, HALFHORIZONTAL, FULLHORIZONTAL }   //  天球,地平半球,地平全天
        private DISPPLANETTYPE mDispPlanetType = DISPPLANETTYPE.HALFHORIZONTAL; //  表示方式(天球/半球)
        private string[] mOpeMenu = {
            "操作メニュー", "北面表示", "東面表示", "南面表示", "西面表示", "全天表示",
            "星座早見盤", "恒星データ", "地平座標データ", "WikiLIst", "太陽系", "恒星データリスト更新"
        };
        private string[] mMaxStarName = { "上限", "1等星", "2等星", "3等星", "4等星", "5等星", "6等星" };
        private string[] mMinStarName = { "下限", "1等星", "2等星", "3等星", "4等星", "5等星", "6等星" };
        private string[] mStarLevel = {
            "全て", "1等星", "2等星", "3等星", "4等星", "5等星", "6等星", "7等星", "8等星", "9等星", "10等星" };
        private int[] mDispNameMag = { -1, 10 };                //  恒星名表示の視等級(最大,最小)
        private int mStarLevelMag = -1;                         //  恒星表示レベル(等級)
        private bool mJpnName = true;                           //  恒星名日本語表示フラグ
        private bool mConstellaLine = false;                    //  星座線表示フラグ
        private bool mConstellaName = false;                    //  星座名表示フラグ
        private bool mNebulaName = false;                       //  星雲・銀河などの表示フラグ
        private bool mPlanet = false;                           //  惑星表示フラグ
        private bool mMilkyway = false;                         //  天の川表示フラグ
        public int mMarkStarData = -1;                          //  リストで選択した恒星No(十字マーク表示)

        private string[] mPlanetName = {
            "水星", "金星", "地球", "火星", "木星", "土星", "天王星", "海王星"
        };
        private Brush[] mPlanetColor = {
            Brushes.Silver, Brushes.Gold, Brushes.Blue, Brushes.Magenta,
            Brushes.Chocolate, Brushes.Yellow, Brushes.Aqua, Brushes.DarkBlue
        };

        private bool mColenderChange = true;                    //  カレンダ変更の有効/無効
        private bool mHourChange = true;                        //  時変更の有効/無効
        private bool mMinuteChange = true;                      //  分変更の有効/無効
        private string[] mLocationTitle = { "札幌", "東京", "大阪", "福岡", "那覇" };
        private PointD[] mLocationData = {
            new PointD(43.06208, 141.35439),                    //  札幌
            new PointD(35.68956, 139.69172),                    //  東京
            new PointD(34.69375, 135.50211),                    //  大阪
            new PointD(33.59014, 130.40172),                    //  福岡
            new PointD(26.21308, 127.67806),                    //  那覇
        };
        private string mLocation = "東京";
        private string mAppName = "PlanetApp";

        private StarDataListView mStarDataListDialog;
        private StarDataListView mDispStarDataListDialog;
        private WikiList mWikiListDialog;                       //  Wikipedia検索ダイヤログ
        private SolarSystem mSoloarSystem;                      //  太陽系ウィンドウ

        private AstroLib alib = new AstroLib();                 //  天体計算、データ処理ライブラリ
        private PlanetLib plib = new PlanetLib();               //  惑星データ
        private YWorldDraw ydraw;                               //  グラフィックライブラリ
        private YLib ylib = new YLib();                         //  単なるライブラリ

        public MainWindow()
        {
            InitializeComponent();

            mWindowWidth = this.Width;
            mWindowHeight = this.Height;
            mPrevWindowWidth = mWindowWidth;

            WindowFormLoad();

            //  実行ファイルのフォルダを取得しワークフォルダとする
            mAppFolder = AppDomain.CurrentDomain.BaseDirectory;             //  アプリケーションフォルダ
            mDataFolder = Path.Combine(mAppFolder, mDataFolder);            //  データファイルフォルダ
            Directory.CreateDirectory(mDataFolder);
            mStarDataOrgPath = Path.Combine(mDataFolder, mStarDataOrgPath); //  恒星元データファイルパス
            mStarDataPath = Path.Combine(mAppFolder, mStarDataPath);        //  恒星データファイルパス

            mStarData = new StarData(mAppFolder, mDataFolder);
            mConstellationData = new ConstellationData(mAppFolder, mDataFolder);
            mStarInfoData = new StarInfoData(mAppFolder, mDataFolder);
            mNebulaData = new NebulaData(mAppFolder, mDataFolder);
            mMilkywayData = new Milkyway(mAppFolder, mDataFolder);

            //  恒星データの読込
            loadStarData(mStarDataPath);
            //  星座データの読込
            loadConstellationData();
            //  星雲、銀河データ
            loadNebulaData();
            //  天の川データ
            loadMilkywayData();

            //  日時データの設定
            setDateMenu();
            //  メニューの登録
            CbMenu.ItemsSource = mOpeMenu;
            CbMenu.SelectedIndex = 0;
            //  日時の設定
            mDateTime = DateTime.Now;
            //  日時設定の登録
            CbHour.Items.Clear();
            for (int h = 0; h < 24; h++)
                CbHour.Items.Add(h.ToString("#時"));
            CbMinute.Items.Clear();
            for (int m = 0; m < 60; m++)
                CbMinute.Items.Add(m.ToString("#分"));
            setDateMenu();
            //  天の川の表示レベル設定
            CbMilkyway.Items.Clear();
            CbMilkyway.Items.Add("天の川");
            for (int i = 100; 0 <= i; i -= 10)
                CbMilkyway.Items.Add(i.ToString());
            CbMilkyway.SelectedIndex = 0;

            //  表示等級
            CbMaxStarName.ItemsSource = mMaxStarName;
            CbMinStarName.ItemsSource = mMinStarName;
            CbMaxStarName.SelectedIndex = 0;
            CbMinStarName.SelectedIndex = 0;
            CbStarLevel.ItemsSource = mStarLevel;
            CbStarLevel.SelectedIndex = 0;
            //  観測点位置
            CbLocation.ItemsSource = mLocationTitle;
            CbLocation.SelectedIndex = mLocationTitle.FindIndex(p => p.CompareTo(mLocation) == 0);

            ydraw = new YWorldDraw(Canvas);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            setDataFile();          //  データファイルリストをコンボボックスに設定
            CbMenu.SelectedIndex = 0;
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
            mPrevWindowWidth = mWindowWidth;

            mWindowState = WindowState;
            drawPlanet(true, new PointD());
            //testWorldDraw();      //  Sample Graphic
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (mStarDataListDialog != null)
                mStarDataListDialog.Close();
            if (mDispStarDataListDialog != null)
                mDispStarDataListDialog.Close();
            if (mWikiListDialog != null)
                mWikiListDialog.Close();
            if (mSoloarSystem != null)
                mSoloarSystem.Close();

            WindowFormSave();
        }

        /// <summary>
        /// Windowの状態を前回の状態にする
        /// </summary>
        private void WindowFormLoad()
        {
            //  前回のWindowの位置とサイズを復元する(登録項目をPropeties.settingsに登録して使用する)
            Properties.Settings.Default.Reload();

            if (Properties.Settings.Default.PlanetWindowWidth < 100 || Properties.Settings.Default.PlanetWindowHeight < 100 ||
                SystemParameters.WorkArea.Height < Properties.Settings.Default.PlanetWindowHeight) {
                Properties.Settings.Default.PlanetWindowWidth = mWindowWidth;
                Properties.Settings.Default.PlanetWindowHeight = mWindowHeight;
            } else {
                Top = Properties.Settings.Default.PlanetWindowTop;
                Left = Properties.Settings.Default.PlanetWindowLeft;
                Width = Properties.Settings.Default.PlanetWindowWidth;
                Height = Properties.Settings.Default.PlanetWindowHeight;
                double dy = Height - mWindowHeight;
            }
            if (0 < Properties.Settings.Default.Location.Length)
                mLocation = Properties.Settings.Default.Location;
        }

        /// <summary>
        /// Window状態を保存する
        /// </summary>
        private void WindowFormSave()
        {
            //  Windowの位置とサイズを保存(登録項目をPropeties.settingsに登録して使用する)
            Properties.Settings.Default.PlanetWindowTop = Top;
            Properties.Settings.Default.PlanetWindowLeft = Left;
            Properties.Settings.Default.PlanetWindowWidth = Width;
            Properties.Settings.Default.PlanetWindowHeight = Height;
            Properties.Settings.Default.Location = mLocation;

            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// [キー入力]処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Left) {
                //  方位を左側に移動
                mDirection -= 0.5;
                mDirection = mDirection < 0 ? mDirection + 24 : mDirection;
                drawPlanet(false, new PointD());
            } else if (e.Key == System.Windows.Input.Key.Right) {
                //  方位を右側に移動
                mDirection += 0.5;
                mDirection = 24 <= mDirection ? mDirection - 24 : mDirection;
                drawPlanet(false, new PointD());
            } else if (e.Key == System.Windows.Input.Key.Up) {
                //  時間を進める
                mDateTime = mDateTime.AddMinutes(5);
                setDateMenu();
                drawPlanet(false, new PointD());
            } else if (e.Key == System.Windows.Input.Key.Down) {
                //  時間を戻す
                mDateTime = mDateTime.AddMinutes(-5);
                setDateMenu();
                drawPlanet(false, new PointD());
            } else if (e.Key == System.Windows.Input.Key.Home) {
                //  時間を現在時にする
                mDateTime = DateTime.Now;
                setDateMenu();
                drawPlanet(false, new PointD());
            } else {
                return;
            }
            btLedt.Focus();     //  矢印キーでコンボボックスにフォーカスが移動するのを防ぐ
        }

        /// <summary>
        /// [メニュー選択]
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (0 <= CbMenu.SelectedIndex && CbMenu.SelectedIndex < mOpeMenu.Length) {
                switch (CbMenu.SelectedIndex) {
                    case 0:         //  メニュータイトル
                        break;
                    case 1:         //  北面表示
                        mDirection = 12;
                        mDispPlanetType = DISPPLANETTYPE.HALFHORIZONTAL;
                        drawPlanet(true, new PointD());
                        break;
                    case 2:         //  東面表示
                        mDirection = 18;
                        mDispPlanetType = DISPPLANETTYPE.HALFHORIZONTAL;
                        drawPlanet(true, new PointD());
                        break;
                    case 3:         //  南面表示
                        mDirection = 0;
                        mDispPlanetType = DISPPLANETTYPE.HALFHORIZONTAL;
                        drawPlanet(true, new PointD());
                        break;
                    case 4:         //  西面表示
                        mDirection = 6;
                        mDispPlanetType = DISPPLANETTYPE.HALFHORIZONTAL;
                        drawPlanet(true, new PointD());
                        break;
                    case 5:         //  全天表示
                        mDispPlanetType = DISPPLANETTYPE.FULLHORIZONTAL;
                        drawPlanet(true, new PointD());
                        break;
                    case 6:         //  天球表示
                        mDispPlanetType = DISPPLANETTYPE.CELESTIAL;
                        drawPlanet(true, new PointD());
                        break;
                    case 7: {       //  恒星データを表形式で表示
                            mStarDataListDialog = new StarDataListView();
                            mStarDataListDialog.mAppFolder = mAppFolder;
                            mStarDataListDialog.mDataFolder = mDataFolder;
                            mStarDataListDialog.mStarDataList = mStarData.mStarStrData;
                            mStarDataListDialog.mMainWindow = null;
                            mStarDataListDialog.Show();
                        }
                        break;
                    case 8: {       //  表示恒星座標データ
                            cnvHorizonStarData();
                            mDispStarDataListDialog = new StarDataListView();
                            mDispStarDataListDialog.Title = Path.GetFileNameWithoutExtension(mStarDataPath);
                            mDispStarDataListDialog.mStarDataList = mHorizonStarData;
                            mDispStarDataListDialog.mStarDataList.Insert(0, mHorizonStarDataTitle);
                            mDispStarDataListDialog.mStatusInfo = mStarDataInfo;       //  日時などの情報
                            mDispStarDataListDialog.mOperation = new string[0];
                            mDispStarDataListDialog.mMainWindow = this;
                            mDispStarDataListDialog.Show();
                        }
                        break;
                    case 9:         //  WikiList Dialog
                        wikiListShow();
                        break;
                    case 10:
                        solarSystemShow();
                        break;
                    case 11:         //  データファイルリスト更新
                        setDataFile();
                        break;
                    default:        //  北面表示
                        mDirection = 0;
                        mDispPlanetType = DISPPLANETTYPE.HALFHORIZONTAL;
                        drawPlanet(true, new PointD());
                        break;
                }
                mMarkStarData = -1;
            }
            CbMenu.SelectedIndex = 0;
        }

        /// <summary>
        /// [表示データの選択]
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbDispFile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (0 <= CbDispFile.SelectedIndex) {
                mStarDataPath = mDataFileList[CbDispFile.SelectedIndex];
                Title = mAppName + " [" + Path.GetFileNameWithoutExtension(mStarDataPath) + "]";
                loadStarData(mStarDataPath);
                drawPlanet(true, new PointD());
            }
        }

        /// <summary>
        /// 天の川の表示レベル選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbMilkyway_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (0 < CbMilkyway.SelectedIndex) {
                mMilkywayDispDinsity = ylib.string2int(CbMilkyway.Items[CbMilkyway.SelectedIndex].ToString());
                mMilkyway = true;
            } else {
                mMilkyway = false;
            }
            drawPlanet(true, new PointD());
        }

        /// <summary>
        /// [場所]の選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (0 <= CbLocation.SelectedIndex) {
                mLocation = CbLocation.Items[CbLocation.SelectedIndex].ToString();
                drawPlanet(true, new PointD());
            }
        }

        /// <summary>
        /// [日付選択] カレンダ表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DpDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mColenderChange) {
                if (setDateTime())
                    drawPlanet(false, new PointD());
            }
            mColenderChange = true;
        }

        /// <summary>
        /// [時選択]
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbHour_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mHourChange) {
                if (setDateTime())
                    drawPlanet(false, new PointD());
            }
            mHourChange = true;
        }

        /// <summary>
        /// [分選択]
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbMinute_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mMinuteChange) {
                if (setDateTime())
                    drawPlanet(false, new PointD());
            }
            mMinuteChange = true;
        }

        /// <summary>
        /// [恒星名の上限視等級]の選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbMaxStarName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (0 <= CbMaxStarName.SelectedIndex) {
                mDispNameMag[0] = CbMaxStarName.SelectedIndex;
                drawPlanet(false, new PointD());
            }
        }

        /// <summary>
        /// [恒星名の下限視等級]の選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbMinStarName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (0 <= CbMinStarName.SelectedIndex) {
                mDispNameMag[1] = CbMinStarName.SelectedIndex;
                drawPlanet(false, new PointD());
            }
        }

        /// <summary>
        /// [恒星の表示レベル]選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbStarLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (0 <= CbStarLevel.SelectedIndex) {
                mStarLevelMag = CbStarLevel.SelectedIndex;
                drawPlanet(false, new PointD());
            }
        }

        /// <summary>
        /// [星座腺]チェック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChConstellaLine_Click(object sender, RoutedEventArgs e)
        {
            mConstellaLine = ChConstellaLine.IsChecked == true;
            drawPlanet(false, new PointD());
        }

        /// <summary>
        /// [星座名]チェック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChConstellaName_Click(object sender, RoutedEventArgs e)
        {
            mConstellaName = ChConstellaName.IsChecked == true;
            drawPlanet(false, new PointD());
        }

        /// <summary>
        /// [星雲・銀河]チェック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbNebula_Click(object sender, RoutedEventArgs e)
        {
            mNebulaName = CbNebula.IsChecked == true;
            drawPlanet(false, new PointD());
        }

        /// <summary>
        /// [惑星]チェック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbPlanet_Click(object sender, RoutedEventArgs e)
        {
            mPlanet = CbPlanet.IsChecked == true;
            drawPlanet(false, new PointD());
        }

        /// <summary>
        /// [左側に回す]ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btLeft_Click(object sender, RoutedEventArgs e)
        {
            mDirection -= 0.5;
            mDirection = mDirection < 0 ? mDirection + 24 : mDirection;
            drawPlanet(false, new PointD());
        }

        /// <summary>
        /// [右側に回す]ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btRight_Click(object sender, RoutedEventArgs e)
        {
            mDirection += 0.5;
            mDirection = 24 <= mDirection ? mDirection - 24 : mDirection;
            drawPlanet(false, new PointD());
        }

        /// <summary>
        /// [時間を戻す]ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btBack_Click(object sender, RoutedEventArgs e)
        {
            mDateTime = mDateTime.AddMinutes(-5);
            setDateMenu();
            drawPlanet(false, new PointD());
        }

        /// <summary>
        /// [現在時刻]ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btNow_Click(object sender, RoutedEventArgs e)
        {
            mDateTime = DateTime.Now;
            setDateMenu();
            drawPlanet(false, new PointD());
        }

        /// <summary>
        /// [時間を進める]ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btForward_Click(object sender, RoutedEventArgs e)
        {
            mDateTime = mDateTime.AddMinutes(5);
            setDateMenu();
            drawPlanet(false, new PointD());
        }

        /// <summary>
        /// [マウスホイール]
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Canvas_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine($"MOuseWheel: {e.Delta}");
            if (0 != e.Delta) {
                PointD pos = screen2Canvas(e.GetPosition(this));
                drawPlanet(false, ydraw.cnvScreen2World(pos), 1.0 - Math.Sign(e.Delta) * 0.1);
            }
        }

        /// <summary>
        /// [マウスの移動]
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //  スクリーン座標をCanvasの座標に
            PointD pos = screen2Canvas(e.GetPosition(this));
            //System.Diagnostics.Debug.WriteLine($"MouseMove {pos.x} {pos.y} ");

        }

        /// <summary>
        /// [左ボタンダウン]
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Canvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            mLeftPressPoint = screen2Canvas(e.GetPosition(this));
            //System.Diagnostics.Debug.WriteLine($"LeftButtonDown {pos.X} {pos.Y} ");

        }

        /// <summary>
        /// [左ボタンアップ]
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Canvas_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //  左ボタンでダウンした位置からの移動
            PointD sp = screen2Canvas(e.GetPosition(this));
            PointD offset = sp.vector(mLeftPressPoint);
            if (10.0 < offset.length()) {
                //  ウィンドウの移動
                PointD wp = ydraw.cnvScreen2World(sp);
                PointD woffset = wp.vector(ydraw.cnvScreen2World(mLeftPressPoint));
                PointD ctr = ydraw.mWorld.getCenter();
                ydraw.setWorldOffset(woffset);
                drawPlanet(false, ctr);
            } else {
                //  データ情報表示
                StarInfoData.SEARCHDATA searchStar = mStarInfoData.searchPos(sp, 5.0);
                if (searchStar != null) {
                    //  指定点の恒星情報を表示
                    PointD ctr = ydraw.mWorld.getCenter();
                    drawPlanet(false, ctr);
                    List<string> infoData = mStarInfoData.searchHipData(searchStar);
                    ydraw.mBrush = Brushes.Black;
                    ydraw.mTextColor = Brushes.Blue;
                    ydraw.mTextSize = mStarInfoTextSize;
                    double y = 0.0;
                    foreach (var text in infoData) {
                        ydraw.drawText(text, 10.0, y);
                        y += ydraw.mTextSize;
                    }
                }
            }
        }

        /// <summary>
        /// [ヘルプ]ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtHelp_Click(object sender, RoutedEventArgs e)
        {
            ylib.openUrl(mHelpFile);
        }

        /// <summary>
        /// スクリーン座標からCanvasの座標に変換
        /// 左と上のコントロール分オフセットする
        /// </summary>
        /// <param name="sp">スクリーン座標</param>
        /// <returns>Cnavas座標</returns>
        private PointD screen2Canvas(Point sp)
        {
            return screen2Canvas(new PointD(sp));
        }

        /// <summary>
        /// スクリーン座標からCanvasの座標に変換
        /// 左と上のコントロール分オフセットする
        /// </summary>
        /// <param name="sp">スクリーン座標</param>
        /// <returns>Cnavas座標</returns>
        private PointD screen2Canvas(PointD sp)
        {
            //Point offset = new Point(SpLeftPanel.ActualWidth, SbTopStatusBar.ActualHeight);
            PointD offset = new PointD(0.0, SbTopStatusBar.ActualHeight);
            sp.offset(-offset.x, -offset.y);
            return sp;
        }

        /// <summary>
        /// データファイルリストをコンボボックスに設定
        /// </summary>
        private void setDataFile()
        {
            //  データファイルリストの登録
            mDataFileList = ylib.getFiles(mAppFolder + "\\*.csv");
            CbDispFile.Items.Clear();
            for (int i = 0; i < mDataFileList.Length; i++) {
                CbDispFile.Items.Add(Path.GetFileName(mDataFileList[i]));
            }
            int index = CbDispFile.Items.IndexOf(Path.GetFileName(mStarDataPath));
            if (0 <= index) {
                CbDispFile.SelectedIndex = index;
                Title = mAppName + " [" + Path.GetFileNameWithoutExtension(mStarDataPath) + "]";
            }
        }

        /// <summary>
        /// 時間データをコンボボックスに設定
        /// </summary>
        private void setDateMenu()
        {
            //  日付時間登録
            mHourChange = false;
            CbHour.SelectedIndex = CbHour.Items.IndexOf(mDateTime.Hour.ToString("#時"));
            mMinuteChange = false;
            CbMinute.SelectedIndex = CbMinute.Items.IndexOf(mDateTime.Minute.ToString("#分"));
            mColenderChange = false;
            DpDate.SelectedDate = mDateTime;
        }

        /// <summary>
        /// 天体表示時間を設定
        /// </summary>
        /// <returns></returns>
        private bool setDateTime()
        {
            if (DpDate.SelectedDate == null || CbHour.SelectedItem == null || CbMinute.SelectedItem == null)
                return false;
            int m = (int)ylib.string2double(CbMinute.SelectedItem.ToString());
            int h = (int)ylib.string2double(CbHour.SelectedItem.ToString());
            DateTime dateTime = DpDate.SelectedDate.Value;
            mDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, h, m, 0);
            return true;
        }

        /// <summary>
        /// 恒星データのプラネタリウム表示
        /// </summary>
        /// <param name="init">初期化</param>
        /// <param name="cp">中心座標の位置</param>
        /// <param name="zoom">ズーミングスケール</param>
        public void drawPlanet(bool init, PointD cp, double zoom = 1.0)
        {
            if (Canvas.ActualWidth == 0 || Canvas.ActualHeight == 0 || CbLocation.SelectedIndex < 0)
                return;

            windowSet(mDispPlanetType, init, cp, zoom);             //  Windowサイズの設定

            //  観測点位置の設定
            mLocalLatitude = mLocationData[CbLocation.SelectedIndex].x;
            mLocalLongitude = mLocationData[CbLocation.SelectedIndex].y;

            //  地方恒星時と観測地緯度の設定
            double gst = getGreenwichSiderealTime();                //  恒星時
            double lst = getLocalSiderealTime();                    //  地方(観測点)恒星時(hour)
            double localLatitude = ylib.D2R(mLocalLatitude);        //  観測点緯度
            mStarDataInfo = mDateTime.ToString() + " GST: " + gst.ToString("#.####")
                + " LST: " + lst.ToString("#.####") + "　緯度: " + mLocalLatitude.ToString("#.####");

            int count = 0;
            mStarInfoData.mSearchData.Clear();

            switch (mDispPlanetType) {
                case DISPPLANETTYPE.CELESTIAL:         //  天球表示
                    drawCelestialBackGround(lst);
                    count = drawCelestialStar();
                    if (mNebulaName)
                        count += drawCelestialNebula();
                    if (mConstellaLine)
                        drawCelestialConstella();
                    if (mConstellaName)
                        drawCelestialConstellaName();
                    break;
                case DISPPLANETTYPE.HALFHORIZONTAL:         //  半円表示
                    drawHorizontalBackGround();
                    if (mMilkyway)
                        drawHorizontalMilkyway(lst, localLatitude);
                    count = drawHorizontalStarData(lst, localLatitude);
                    if (mNebulaName)
                        count += drawHorizontalNebula(lst, localLatitude);
                    if (mConstellaLine)
                        drawHorizontalConstellaLine(lst, localLatitude);
                    if (mConstellaName)
                        drawHorizontalConstellaName(lst, localLatitude);
                    if (mPlanet)
                        drawHorizontalPlanet(lst, localLatitude);
                    break;
                case DISPPLANETTYPE.FULLHORIZONTAL:         //  全天表示
                    drawFullHorizontalBackGround();
                    if (mMilkyway)
                        drawHorizontalMilkyway(lst, localLatitude, true);
                    count = drawHorizontalStarData(lst, localLatitude, true);
                    if (mNebulaName)
                        count += drawHorizontalNebula(lst, localLatitude, true);
                    if (mConstellaLine)
                        drawHorizontalConstellaLine(lst, localLatitude, true);
                    if (mConstellaName)
                        drawHorizontalConstellaName(lst, localLatitude, true);
                    if (mPlanet)
                        drawHorizontalPlanet(lst, localLatitude, true);
                    break;
            }

            TbInfo.Text = "星の数: " + count.ToString("#,###") + " [" + mStarDataInfo + "]";
        }

        /// <summary>
        /// ワールドウィンドウの設定
        /// </summary>
        /// <param name="windowType">表示形式(0:全円 1:半円))</param>
        /// <param name="init">初期化</param>
        /// <param name="cp">中心座標の位置</param>
        /// <param name="zoom">ズーミングスケール</param>
        private void windowSet(DISPPLANETTYPE windowType, bool init, PointD cp, double zoom = 1.0)
        {
            //  Window設定
            if (!init && (cp.isEmpty() || cp.isNaN()))
                cp = ydraw.mWorld.getCenter();

            if (init) {
                //  View初期化
                ydraw.setViewArea(0.0, 0.0, Canvas.ActualWidth, Canvas.ActualHeight);   //  Viewの設定
                ydraw.mAspectFix = true;        //  アスペクト比保持
                ydraw.mClipping = true;         //  クリッピング可
                if (windowType == DISPPLANETTYPE.CELESTIAL) {               //  全炎(天球面)
                    ydraw.setWorldWindow(-mPlanetWindowSize, mPlanetWindowSize, mPlanetWindowSize, -mPlanetWindowSize);
                } else if (windowType == DISPPLANETTYPE.HALFHORIZONTAL) {   //  半円
                    ydraw.setWorldWindow(-mPlanetWindowSize, mPlanetWindowSize, mPlanetWindowSize, -mPlanetWindowSize * 0.05);
                } else if (windowType == DISPPLANETTYPE.FULLHORIZONTAL) {   //  全天
                    ydraw.setWorldWindow(-mPlanetWindowSize *1.1, mPlanetWindowSize * 1.1, mPlanetWindowSize * 1.1, -mPlanetWindowSize * 1.1);
                }
            } else {
                ydraw.setWorldZoom(cp, zoom);
            }
        }

        /// <summary>
        /// 全天表示の背景設定
        /// </summary>
        private void drawFullHorizontalBackGround()
        {
            ydraw.clear();
            ydraw.mFillColor = mBaseBackColor;
            ydraw.drawWRectangle(ydraw.mWorld.ToRect());

            ydraw.mThickness = 1.0;
            ydraw.mBrush = mBackGroundBorderColor;
            ydraw.mFillColor = mBackGroundColor;
            ydraw.drawWCircle(new PointD(0, 0), mCelestialRadius);

            //  高度補助線
            double height = Math.PI / 9.0;
            ydraw.mFillColor = null;
            ydraw.mBrush = mAuxLineColor;
            while (height < Math.PI / 2) {
                ydraw.drawWCircle(new PointD(0, 0), mCelestialRadius * (1.0 - height / (Math.PI / 2.0)));
                height += Math.PI / 9.0;
            }
            //  方位補助線
            double scaleOffset = Math.PI * ydraw.screen2worldXlength(mScaleTextSize) / mCelestialRadius / 2.0 * 1.5;
            double largeScaleOffset = Math.PI * ydraw.screen2worldXlength(mScaleLargeTextSize) / mCelestialRadius / 2.0;
            for (int hour = 0; hour < 24; hour++) {
                if (hour % 2 == 0) {
                    ydraw.mBrush = mAuxLineColor;
                    ydraw.drawWLine(new PointD(0, 0), alib.cnvFullHorizontal(ylib.H2R(hour), 0.0, mDirection, mCelestialRadius));
                    ydraw.mBrush = mScaleTextColor;
                    ydraw.mTextSize = mScaleTextSize;
                    PointD p = alib.cnvFullHorizontal(ylib.H2R(hour), -scaleOffset / 2.0, mDirection, mCelestialRadius);
                    ydraw.drawWText((hour % 24).ToString("00h"), p, 0, 0, HorizontalAlignment.Center, VerticalAlignment.Center);
                }
                if (hour % 6 == 0) {
                    ydraw.mBrush = mScaleTextColor;
                    ydraw.mTextSize = mScaleLargeTextSize;
                    PointD p = alib.cnvFullHorizontal(ylib.H2R(hour), -scaleOffset - largeScaleOffset / 2.0, mDirection, mCelestialRadius);
                    ydraw.drawWText(mAzimuthName[ylib.mod((int)(hour / 6), 4)], p, 0, 0, HorizontalAlignment.Center, VerticalAlignment.Center);
                }
                ydraw.mBrush = mScaleOutlineColor;
                ydraw.drawWLine(alib.cnvFullHorizontal(ylib.H2R(hour), Math.PI / 36.0, mDirection, mCelestialRadius),
                    alib.cnvFullHorizontal(ylib.H2R(hour), 0.0, mDirection, mCelestialRadius));
            }
        }

        /// <summary>
        /// 天球面(星座早見盤)の赤経・赤緯表示
        /// </summary>
        /// <param name="lst">地方恒星時(hour)</param>
        private void drawCelestialBackGround(double lst)
        {
            ydraw.clear();
            ydraw.mFillColor = mBaseBackColor;
            ydraw.drawWRectangle(ydraw.mWorld.ToRect());

            ydraw.mThickness = 1.0;
            ydraw.mBrush = mBackGroundBorderColor;
            ydraw.mFillColor = mBackGroundColor;
            ydraw.drawWCircle(new PointD(0, 0), mCelestialRadius);

            //  赤緯の補助線(度)
            for (int i = 50; -90 < i; i -= 40) {
                double l = alib.declinationLength(ylib.D2R(i), mCelestialRadius);
                ydraw.mBrush = mAuxLineColor;
                ydraw.drawWCircle(new PointD(0, 0), l);
                ydraw.mTextSize = mScaleTextSize;
                ydraw.mBrush = mScaleTextColor;
                ydraw.drawWText(i.ToString("##°"), new PointD(l, 0.0), 0, 0, HorizontalAlignment.Center, VerticalAlignment.Bottom);
            }

            //  赤経の補助線(時)
            for (int ra = 0; ra < 24; ra += 1) {
                PointD ps = alib.equatorial2orthogonal(new PointD(ylib.H2R(ra), ylib.D2R(-90)), mDirection, mCelestialRadius);
                PointD pe = new PointD(0, 0);
                LineD line = new LineD(ps, pe);
                if (ra % 2 == 0) {
                    //  補助線と目盛り
                    ydraw.mBrush = mScaleTextColor;
                    PointD p = alib.equatorial2orthogonal(new PointD(ylib.H2R(ra), ylib.D2R(-90 * 1.1)), mDirection, mCelestialRadius);
                    ydraw.drawWText((ra % 24).ToString("00h"), p, 0, 0, HorizontalAlignment.Center, VerticalAlignment.Center);
                    ydraw.mBrush = mAuxLineColor;
                    ydraw.drawWLine(line);
                }
                //  目盛り線
                ydraw.mBrush = mScaleOutlineColor;
                line.setLength(mCelestialRadius * 0.05);
                ydraw.drawWLine(line);
            }

            //  地平線表示
            double hour = 0.0;
            double height = 0.0;                //  高度(rad)
            double azimuth = ylib.H2R(hour);    //  方位(rad)
            PointD hps = alib.equatorial2orthogonal(new PointD(
                alib.equatorialRightAscension(azimuth, height, ylib.D2R(mLocalLatitude), ylib.H2R(lst)),
                alib.equatorialDeclination(azimuth, height, ylib.D2R(mLocalLatitude))), 
                mDirection, mCelestialRadius);
            PointD hpe;
            while (hour < 24.0) {
                if (hour % 6 == 0) {
                    //  東西南北表示
                    ydraw.mTextSize = mScaleLargeTextSize;
                    ydraw.mTextColor = mHorizontalTextColor;
                    ydraw.drawWText(mAzimuthName[(int)(hour / 6) % 4], hps, 0, 0, HorizontalAlignment.Center, VerticalAlignment.Center);
                }
                hour += 0.5;
                azimuth = ylib.H2R(hour);
                hpe = alib.equatorial2orthogonal(new PointD(
                    alib.equatorialRightAscension(azimuth, height, ylib.D2R(mLocalLatitude), ylib.H2R(lst)),
                    alib.equatorialDeclination(azimuth, height, ylib.D2R(mLocalLatitude))), 
                    mDirection, mCelestialRadius);
                ydraw.mBrush = mHorizontalLineColor;
                ydraw.drawWLine(hps,hpe);
                hps = hpe;
            }
        }

        /// <summary>
        /// 天球面(星座早見盤)での恒星表示
        /// </summary>
        /// <returns>表示数</returns>
        private int drawCelestialStar()
        {
            int count = 0;
            if (mStarData.mStarData == null)
                return count;

            foreach (StarData.STARDATA star in mStarData.mStarData) {
                PointD p = alib.equatorial2orthogonal(star.coordinate, mDirection, mCelestialRadius);
                if (p.length() < mCelestialRadius && (mStarLevelMag == 0 ||star.magnitude < mStarLevelMag)) {
                    drawStar(p, star);
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 天球面(星座早見盤)での星雲・銀河表示
        /// </summary>
        /// <returns>表示数</returns>
        private int drawCelestialNebula()
        {
            int count = 0;
            if (mNebulaData.mNebulaData == null)
                return count;

            ydraw.mBrush = mNeburaBorderColor;
            ydraw.mTextSize = mStarNemeTextSize;
            ydraw.mTextColor = mNebulaNamColor;
            ydraw.mFillColor = mStarFillColor;

            foreach (NebulaData.NEBULADATA nebula in mNebulaData.mNebulaData) {
                PointD p = alib.equatorial2orthogonal(nebula.coordinate, mDirection, mCelestialRadius);
                if (p.length() < mCelestialRadius && (mStarLevelMag == 0 || nebula.magnitude < mStarLevelMag)) {
                    drawNebula(p, nebula);
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 天球面(星座早見盤)での星座線表示
        /// </summary>
        private void drawCelestialConstella()
        {
            if (mConstellationData.mConstellaLineLineList == null)
                return;

            ydraw.mBrush = mConstellaLineColor;
            ydraw.mThickness = 1.0;
            foreach (ConstellationData.CONSTELLATIONLINE costellaLine in mConstellationData.mConstellaLineLineList) {
                if (!mConstellationData.mConstellaStarList.ContainsKey(costellaLine.sHip) ||
                    !mConstellationData.mConstellaStarList.ContainsKey(costellaLine.eHip))
                    continue;
                ConstellationData.CONSTELLATIONSTAR sStar = mConstellationData.mConstellaStarList[costellaLine.sHip];
                ConstellationData.CONSTELLATIONSTAR eStar = mConstellationData.mConstellaStarList[costellaLine.eHip];
                if (ylib.D2R(-80) < sStar.coordinate.y && ylib.D2R(-80) < eStar.coordinate.y) {
                    PointD sp = alib.equatorial2orthogonal(sStar.coordinate, mDirection, mCelestialRadius);
                    PointD ep = alib.equatorial2orthogonal(eStar.coordinate, mDirection, mCelestialRadius);
                    ydraw.drawWLine(sp, ep);
                }
            }
        }

        /// <summary>
        /// 天球面(星座早見盤)での星座名表示
        /// </summary>
        private void drawCelestialConstellaName()
        {
            if (mConstellationData.mConstellaNameList == null)
                return;

            ydraw.mTextColor = mConstellaNameColor;
            ydraw.mTextSize = mStarNemeTextSize;
            foreach (ConstellationData.CONSTELLATIONNAME constellaName in mConstellationData.mConstellaNameList) {
                PointD p = alib.equatorial2orthogonal(constellaName.coordinate, mDirection, mCelestialRadius);
                if (p.length() < mCelestialRadius) {
                    drawConstellaName(p, constellaName);
                }
            }
        }

        /// <summary>
        /// 地平座標系での背景表示(半円)
        /// </summary>
        private void drawHorizontalBackGround()
        {
            ydraw.clear();
            ydraw.mFillColor = mBaseBackColor;
            ydraw.drawWRectangle(ydraw.mWorld.ToRect());

            //  背景表示
            ydraw.mThickness = 1.0;
            ydraw.mBrush = mBackGroundBorderColor;
            ydraw.mFillColor = mBackGroundColor;
            ydraw.drawWArc(new PointD(0, 0), mCelestialRadius, 0.0, Math.PI);
            ydraw.drawWLine(new PointD(mCelestialRadius, 0.0), new PointD(-mCelestialRadius, 0.0));

            //  目盛り表示 (経度線)
            double azStep = 1.0;
            double sp = mDirection - 6.0;
            double ep = sp + 12;
            double az = Math.Ceiling(sp / azStep) * azStep;
            while (az <= ep) {
                PointD ps = alib.cnvHorizontal(ylib.H2R(az), 0.0, mDirection, mCelestialRadius);
                PointD pe = alib.cnvHorizontal(ylib.H2R(az), 0.05, mDirection, mCelestialRadius);
                LineD line = new LineD(ps, pe);
                if (az % 2 == 0) {
                    //  方位を時表示
                    ydraw.mTextSize = mScaleTextSize;
                    ydraw.mTextColor = mScaleTextColor;
                    ydraw.drawWText((az % 24).ToString("00h"), ps, 0, 0, HorizontalAlignment.Center, VerticalAlignment.Top);
                    //  補助線
                    ydraw.mBrush = mAuxLineColor;
                    PointD p = ps;
                    for (int h = 10; h <= 90; h += 10) {
                        PointD tp = alib.cnvHorizontal(ylib.H2R(az), ylib.D2R(h), mDirection, mCelestialRadius);
                        ydraw.drawWLine(p, tp);
                        p = tp;
                    }
                }
                if (az % 6 == 0) {
                    //  東西南北表示
                    ydraw.mTextSize = mScaleLargeTextSize;
                    ydraw.mTextColor = mScaleLargeTextColor;
                    PointD p = new PointD(ps.x, ps.y - ydraw.screen2worldXlength(20));
                    ydraw.drawWText(mAzimuthName[ylib.mod((int)(az / 6), 4)], p, 0, 0, HorizontalAlignment.Center, VerticalAlignment.Top);
                }
                ydraw.mBrush = mScaleOutlineColor;
                ydraw.drawWLine(line);
                az += azStep;
            }
            //  高度の補助線
            int latiStep = 20;      //  高度線の間隔(deg)
            azStep = 1.0;
            for (int lati = latiStep; lati < 90; lati += latiStep) {
                //  高度値(deg)表示
                ydraw.mTextSize = mScaleTextSize;
                ydraw.mTextColor = mScaleTextColor;
                PointD lp = alib.cnvHorizontal(ylib.H2R(sp), ylib.D2R(lati), mDirection, mCelestialRadius);
                ydraw.drawWText(lati.ToString("00°"), lp, 0, 0, HorizontalAlignment.Right, VerticalAlignment.Center);
                //  高度の補助線
                az = sp;
                while (az < ep) {
                    PointD ps = alib.cnvHorizontal(ylib.H2R(az), ylib.D2R(lati), mDirection, mCelestialRadius);
                    PointD pe = alib.cnvHorizontal(ylib.H2R((az + azStep) % 24.0), ylib.D2R(lati), mDirection, mCelestialRadius);
                    ydraw.mBrush = mAuxLineColor;
                    ydraw.drawWLine(ps, pe);
                    az += azStep;
                }
            }
        }

        /// <summary>
        /// 地平座標系での恒星データ表示
        /// </summary>
        /// <param name="lst">地方恒星時</param>
        /// <param name="localLatitude">観測点緯度</param>
        /// <param name="full">全天表示(円/半円)</param>
        /// <returns>恒星の表示数</returns>
        private int drawHorizontalStarData(double lst, double localLatitude, bool full = false)
        {
            int count = 0;
            if (mStarData.mStarData == null)
                return count;

            ydraw.mTextSize = mStarNemeTextSize;
            foreach (StarData.STARDATA star in mStarData.mStarData) {
                PointD p = convHorizontalPoint(star.coordinate, lst, localLatitude, full);
                if (!p.isEmpty() && !p.isNaN()) {
                    if (p.length() < mCelestialRadius && (mStarLevelMag == 0 || star.magnitude < mStarLevelMag)) {
                        drawStar(p, star);
                        count++;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// 地平座標で星雲・銀河などを表示
        /// </summary>
        /// <param name="lst">地方恒星時</param>
        /// <param name="localLatitude">観測点緯度</param>
        /// <param name="full">全天表示(円/半円)</param>
        /// <returns>表示数</returns>
        private int drawHorizontalNebula(double lst, double localLatitude, bool full = false)
        {
            int count = 0;
            if (mNebulaData.mNebulaData == null)
                return count;

            ydraw.mBrush = mNeburaBorderColor;
            ydraw.mFillColor = mStarFillColor;
            ydraw.mTextColor = mNebulaNamColor;
            ydraw.mTextSize = mStarNemeTextSize;
            foreach (NebulaData.NEBULADATA nebula in mNebulaData.mNebulaData) {
                PointD p = convHorizontalPoint(nebula.coordinate, lst, localLatitude, full);
                if (!p.isEmpty() && !p.isNaN()) {
                    if (p.length() < mCelestialRadius && (mStarLevelMag == 0 || nebula.magnitude < mStarLevelMag)) {
                        drawNebula(p, nebula);
                        count++;
                    }
                }
            }

            return count;
        }


        /// <summary>
        /// 地平座標系での星座線表示
        /// <param name="lst">地方恒星時</param>
        /// <param name="localLatitude">観測点緯度</param>
        /// <param name="full">全天表示(円/半円)</param>
        /// </summary>
        private void drawHorizontalConstellaLine(double lst, double localLatitude, bool full = false)
        {
            if (mConstellationData.mConstellaLineLineList == null)
                return;

            ydraw.mBrush = mConstellaLineColor;
            ydraw.mThickness = 1.0;
            foreach (ConstellationData.CONSTELLATIONLINE costellaLine in mConstellationData.mConstellaLineLineList) {
                if (!mConstellationData.mConstellaStarList.ContainsKey(costellaLine.sHip) ||
                    !mConstellationData.mConstellaStarList.ContainsKey(costellaLine.eHip))
                    continue;
                ConstellationData.CONSTELLATIONSTAR sStar = mConstellationData.mConstellaStarList[costellaLine.sHip];
                ConstellationData.CONSTELLATIONSTAR eStar = mConstellationData.mConstellaStarList[costellaLine.eHip];
                PointD sp = convHorizontalPoint(sStar.coordinate, lst, localLatitude, full);
                PointD ep = convHorizontalPoint(eStar.coordinate, lst, localLatitude, full);
                if (!sp.isEmpty() && !ep.isEmpty() && !sp.isNaN() && !ep.isNaN()) {
                    ydraw.drawWLine(sp, ep);
                }
            }
        }

        /// <summary>
        /// 地平座標系で星座名の表示
        /// </summary>
        /// <param name="lst">地方恒星時</param>
        /// <param name="localLatitude">観測点緯度</param>
        /// <param name="full">全天表示(円/半円)</param>
        private void drawHorizontalConstellaName(double lst, double localLatitude, bool full = false)
        {
            if (mConstellationData.mConstellaNameList == null)
                return;

            ydraw.mTextColor = mConstellaNameColor;
            ydraw.mTextSize = mStarNemeTextSize;
            foreach (ConstellationData.CONSTELLATIONNAME constellaName in mConstellationData.mConstellaNameList) {
                PointD p = convHorizontalPoint(constellaName.coordinate, lst, localLatitude, full);
                if (!p.isEmpty() && !p.isNaN()) {
                    drawConstellaName(p, constellaName);
                }
            }
        }

        /// <summary>
        /// 惑星の表示
        /// </summary>
        /// <param name="lst">地方恒星時</param>
        /// <param name="localLatitude">観測点緯度</param>
        /// <param name="full">全天表示(円/半円)</param>
        private void drawHorizontalPlanet(double lst, double localLatitude, bool full = false)
        {
            ydraw.mTextColor = mConstellaNameColor;
            ydraw.mTextSize = mStarNemeTextSize;
            //  ユリウス日
            double jd = ylib.getJD(mDateTime.Year, mDateTime.Month, mDateTime.Day, mDateTime.Hour, mDateTime.Minute, mDateTime.Second);
            for (int i = 0; i < mPlanetName.Length; i++) { 
                if (mPlanetName[i].CompareTo("地球") != 0) {
                    PointD planetPos = plib.equatorialCoordinate(mPlanetName[i], jd);   //  赤道座標
                    if (planetPos.isEmpty() || planetPos.isNaN())
                        continue;
                    PointD p = convHorizontalPoint(planetPos, lst, localLatitude, full);    //  地平座標に変換
                    if (p.isEmpty() || p.isNaN())
                        continue;
                    //  惑星の表示
                    ydraw.mFillColor = mPlanetColor[i];
                    ydraw.mBrush = mStarBorderColor;
                    ydraw.drawWCircle(p, ydraw.screen2worldXlength(magnitude2radius(1)));
                    //  惑星名の表示
                    ydraw.mTextSize = mStarNemeTextSize;
                    ydraw.mTextColor = mStarNameColor;
                    ydraw.drawWText(mPlanetName[i], p);
                }
            }
            //  月の表示
            PointD moonEcliptic = plib.moonEclipticCoordinate(jd);  //  黄経・黄緯
            PointD moonEquatorialPos = plib.ecliptic2equatorial(moonEcliptic, plib.getEpslion(jd));
            PointD mp = convHorizontalPoint(moonEquatorialPos, lst, localLatitude, full);
            if (!mp.isEmpty() && !mp.isNaN()) {
                ydraw.mFillColor = Brushes.Yellow;
                ydraw.mBrush = mStarBorderColor;
                ydraw.drawWCircle(mp, ydraw.screen2worldXlength(magnitude2radius(1)));
                ydraw.mTextSize = mStarNemeTextSize;
                ydraw.mTextColor = mStarNameColor;
                ydraw.drawWText("月", mp);
            }
        }

        /// <summary>
        /// 天の川のデータ表示
        /// </summary>
        /// <param name="lst">地方恒星時</param>
        /// <param name="localLatitude">観測点緯度</param>
        /// <param name="full">全天表示(円/半円)</param>
        private void drawHorizontalMilkyway(double lst, double localLatitude, bool full = false)
        {
            if (mMilkywayData.mMilkywayData == null)
                return;
            ydraw.mBrush = Brushes.White;
            ydraw.mThickness = 1.0;
            //double ex = ydraw.screen2worldXlength(5);
            int ex = (int)Math.Max(ydraw.world2screenXlength(0.35), 1.0);
            ydraw.mPointSize = ex;
            //double ex = 0.2;
            for (int i = 0; i < mMilkywayData.mMilkywayData.Count; i++) {
                PointD p = convHorizontalPoint(mMilkywayData.mMilkywayData[i].coordinate, lst, localLatitude, full);
                if (!p.isEmpty() && !p.isNaN() &&
                    (i % 10 < (mMilkywayDispDinsity / 10))) {           //  配列で間引く
                    //((100 - mMilkywayDispDinsity) / 2 < mMilkywayData.mMilkywayData[i].density)) {  //  濃度で間引く
                    ydraw.drawWPoint(p);
                    //PointD pe = p.toCopy();
                    //pe.Offset(ex, ex);
                    //ydraw.drawWLine(p, pe);
                }
            }

        }


        /// <summary>
        /// 恒星データ表示
        /// </summary>
        /// <param name="p">位置(直交座標)</param>
        /// <param name="star">恒星データ</param>
        private void drawStar(PointD p, StarData.STARDATA star)
        {
            //  画面位置の登録
            mStarInfoData.mSearchData.Add(new StarInfoData.SEARCHDATA(ydraw.cnvWorld2Screen(p), star.starName, -1, star.hipNo));
            //  マークの十字表示
            if (0 <= mMarkStarData && mStarData.mStarData[mMarkStarData] == star) {
                ydraw.mBrush = mMarckColor;
                double exLine = ydraw.screen2worldXlength(15);
                ydraw.drawWLine(new PointD(p.x + exLine, p.y), new PointD(p.x - exLine, p.y));
                ydraw.drawWLine(new PointD(p.x, p.y + exLine), new PointD(p.x, p.y - exLine));
            }
            //  恒星の円表示
            ydraw.mFillColor = mStarFillColor;
            ydraw.mBrush = mStarBorderColor;
            ydraw.drawWCircle(p, ydraw.screen2worldXlength(magnitude2radius(star.magnitude)));
            //  恒星名表示
            if ((mDispNameMag[0] == 0 || mDispNameMag[0] < star.magnitude)
                && (mDispNameMag[1] == 0 || star.magnitude < mDispNameMag[1] + 1)) {
                ydraw.mTextSize = mStarNemeTextSize;
                ydraw.mTextColor = mStarNameColor;
                if (0 < star.starNameJp.Length && mJpnName)
                    ydraw.drawWText(star.starNameJp, p);
                else if (0 < star.starName.Length)
                    ydraw.drawWText(star.starName, p);
            }
        }

        /// <summary>
        /// 星雲・銀河などの表示
        /// </summary>
        /// <param name="p">位置(直交座標)</param>
        /// <param name="nebula">星雲・銀河などのデータ</param>
        private void drawNebula(PointD p, NebulaData.NEBULADATA nebula)
        {
            mStarInfoData.mSearchData.Add(new StarInfoData.SEARCHDATA(ydraw.cnvWorld2Screen(p), nebula.name, nebula.messierNo, -1));
            ydraw.drawWCircle(p, ydraw.screen2worldXlength(magnitude2radius(nebula.magnitude / 2.0)));
            //  星雲・銀河などの表示
            if (mNebulaName) {
                string name = "NGC" + nebula.NGCNo + " " + nebula.name;
                ydraw.drawWText(name, p);
            }
        }

        /// <summary>
        /// 星座名の表示
        /// </summary>
        /// <param name="p">位置(直交座標)</param>
        /// <param name="constellaName">星座名</param>
        private void drawConstellaName(PointD p, ConstellationData.CONSTELLATIONNAME constellaName)
        {
            if (0 < constellaName.constrationNameJpn.Length)
                ydraw.drawWText(constellaName.constrationNameJpn, p);
            else if (0 < constellaName.constrationName.Length)
                ydraw.drawWText(constellaName.constrationName, p);
            else if (0 < constellaName.constrationNameMono.Length)
                ydraw.drawWText(constellaName.constrationNameMono, p);
        }

        /// <summary>
        /// 恒星時(時)の取得
        /// </summary>
        /// <returns>恒星時(hh.hhhh)</returns>
        private double getGreenwichSiderealTime()
        {
            return ylib.getGreenwichSiderealTime(
                mDateTime.Year, mDateTime.Month, mDateTime.Day,
                mDateTime.Hour, mDateTime.Minute, mDateTime.Second);
        }

        /// <summary>
        /// 地方恒星時(時)と観測点緯度の取得(rad)
        /// </summary>
        /// <returns>(地方恒星時(hh.hhhh),観測点緯度(rad))</returns>
        private double getLocalSiderealTime()
        {
            double gst = ylib.getGreenwichSiderealTime(
                mDateTime.Year, mDateTime.Month, mDateTime.Day,
                mDateTime.Hour, mDateTime.Minute, mDateTime.Second);
            double lst = ylib.getLocalSiderealTime(gst, mLocalLongitude - 135.0);
            return lst;
        }

        /// <summary>
        /// 赤道座標系を地平座標系に変換
        /// </summary>
        /// <param name="coordinate">赤道座標(rad)</param>
        /// <param name="lst">地方恒星時(時)</param>
        /// <param name="localLatitude">観測点緯度(rad)</param>
        /// <param name="full">全天表示(円/半円)の座標変換</param>
        /// <returns>(表示可否,地平座標(rad))</returns>
        private PointD convHorizontalPoint(PointD coordinate, double lst, double localLatitude, bool full = false)
        {
            double hourAngle = ylib.H2R(lst - ylib.R2H(coordinate.x));                  //  時角
            double height = alib.horizonHeight(hourAngle, coordinate.y, localLatitude); //  高さ
            double azimuth = alib.horizonAzimuth(hourAngle, coordinate.y, localLatitude);//  方位

            PointD p = new PointD(0, 0);
            double sp = (mDirection - 6 + 24) % 24.0;               //  方位の表示開始角度(常に正の値))
            double hazimuth = (ylib.R2H(azimuth) - sp + 24)  % 24;  //  表示開始位置と星の位置の方位差)
            if (0 < height && (full || (0 < hazimuth && hazimuth < 12))) {
                if (full)
                    p = alib.cnvFullHorizontal(azimuth, height, mDirection, mCelestialRadius);
                else
                    p = alib.cnvHorizontal(azimuth, height, mDirection, mCelestialRadius);
                if (p.length() > mCelestialRadius)
                    p.clear();
            }
            return p;
        }


        /// <summary>
        /// 視等級を半径に変換する
        /// </summary>
        /// <param name="magnitude">視等級</param>
        /// <returns>半径</returns>
        private double magnitude2radius(double magnitude)
        {
            return Math.Sqrt(3 - (magnitude > 3 ? 2.5 : magnitude)) * mStarRadiusRate;
        }

        /// <summary>
        /// 地平座標系での恒星データをテキストデータリストに変換
        /// </summary>
        private void cnvHorizonStarData()
        {
            double gst = ylib.getGreenwichSiderealTime(
                            mDateTime.Year, mDateTime.Month, mDateTime.Day,
                            mDateTime.Hour, mDateTime.Minute, mDateTime.Second);
            double lst = ylib.getLocalSiderealTime(gst, mLocalLongitude - 135.0);
            double localLatitude = ylib.D2R(mLocalLatitude);
            string info = mDateTime.ToString() + " GST: " + gst.ToString() + " LST: " + lst +
                            "　緯度: " + mLocalLatitude.ToString();
            mStarDataInfo = info;
            mHorizonStarDataTitle = new string[] {
                "HIP番号", "赤経", "赤緯", "方位", "高度", "時角", "視等級", "恒星名", "恒星名(日本語)"
            };
            mHorizonStarData = new List<string[]>();

            foreach (StarData.STARDATA star in mStarData.mStarData) {
                double hourAngle = ylib.H2R(lst - ylib.R2H(star.coordinate.x));
                double height = alib.horizonHeight(hourAngle, star.coordinate.y, localLatitude);
                double azimuth = alib.horizonAzimuth(hourAngle, star.coordinate.y, localLatitude);
                string[] buf = new string[mHorizonStarDataTitle.Length];
                buf[0] = star.hipNo.ToString("000000");
                buf[1] = ylib.hour2HMS(ylib.R2H(star.coordinate.x));
                buf[2] = ylib.deg2DMS(ylib.R2D(star.coordinate.y));
                buf[3] = ylib.hour2HMS(ylib.R2H(azimuth));
                buf[4] = ylib.deg2DMS(ylib.R2D(height));
                buf[5] = ylib.hour2HMS(ylib.R2H(hourAngle));
                buf[6] = star.magnitude.ToString();
                buf[7] = star.starName;
                buf[8] = star.starNameJp;
                mHorizonStarData.Add(buf);
            }
        }

        /// <summary>
        /// 恒星データの読込
        /// </summary>
        private void loadStarData(string starDataPath)
        {
            ylib.mEncordingType = 1;                            //  shift-jisを指定
            if (File.Exists(starDataPath)) {
                if (mStarData.loadStarData(starDataPath)) {
                    mStarData.convStanderdData();
                    mStarData.convStarData();
                    mStarInfoData.loadInfoData(mStarData.mOrgStarStrData);
                }
            }
        }

        /// <summary>
        /// 星座データの読込
        /// </summary>
        private void loadConstellationData()
        {
            ylib.mEncordingType = 1;                            //  shift-jisを指定
            mConstellationData.loadConstellationLine();
            mConstellationData.loadConstellationStar();
            mConstellationData.loadConstellationName();
        }

        /// <summary>
        /// 星雲。銀河などのデータ読込
        /// </summary>
        private void loadNebulaData()
        {
            mNebulaData.loadData();
            mStarInfoData.mNebulaStrData = mNebulaData.mNebulaStrData;
        }

        /// <summary>
        /// 天の川データの読込
        /// </summary>
        private void loadMilkywayData()
        {
            mMilkywayData.loadData();
        }

        /// <summary>
        /// WikkListのダイヤログ表示
        /// </summary>
        private void wikiListShow()
        {
            mWikiListDialog = new WikiList();
            //mWikiListDialog.Topmost = true;
            mWikiListDialog.mMainWindow = this;
            mWikiListDialog.Show();
        }

        /// <summary>
        /// 惑星の軌道表示
        /// </summary>
        private void solarSystemShow()
        {
            mSoloarSystem = new SolarSystem();
            mSoloarSystem.Show();
        }
    }
}

