using CoreLib;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace PlanetApp
{
    /// <summary>
    /// SolarSystem.xaml の相互作用ロジック
    /// </summary>
    public partial class SolarSystem : Window
    {
        private double mWindowWidth;                            //  ウィンドウの高さ
        private double mWindowHeight;                           //  ウィンドウ幅
        private double mPrevWindowWidth;                        //  変更前のウィンドウ幅
        private WindowState mWindowState = WindowState.Normal;  //  ウィンドウの状態(最大化/最小化)

        private Brush mBaseBackColor = Brushes.White;           //  全体の背景色

        private double mSolarSystemSize = 2.0;                  //  太陽系の表示範囲(AU)
        private string[] mPlanetType = { 
            "太陽中心軌道", "地球中心軌道" };
        private string[] mStepDayTutle = { 
            "ステップ日数", "1日", "2日", "3日", "5日", "10日", "20日", "30日" };
        private string[] mPlanetName = {
            "水星", "金星", "地球", "火星", "木星", "土星", "天王星", "海王星"
        };
        private Brush[] mPlanetColor = {
            Brushes.Silver, Brushes.Gold, Brushes.Blue, Brushes.Magenta,
            Brushes.Chocolate, Brushes.Yellow, Brushes.Aqua, Brushes.DarkBlue
        };
        private DateTime mDateTime;                             //  表示日時
        private int mDateIncrimentSize = 3;                     //  1クリックの更新日数
        private double mRoollX = 0;                             //  X軸の回転角度(deg)
        private double mRoollY = 0;                             //  X軸の回転角度(deg)
        private bool mPtolemaic = false;                        //  天動説(地球中心)表示
        private List<List<Point3D>> mPtlemaicList;              //  惑星の軌跡データ

        private DispatcherTimer dispatcherTimer;                //  惑星運行の自動実行
        private int mSecInterval = 0;                           //  自動実行時のインターバル秒
        private int mMiliInterval = 300;                        //  自動実行時のインターバルm秒
        private PointD mLeftPressPoint = new PointD();          //  マウス左ボタン位置

        private PlanetLib plib = new PlanetLib();               //  惑星データ
        private YWorldDraw ydraw;                               //  グラフィックライブラリ
        private YLib ylib = new YLib();                         //  単なるライブラリ

        public SolarSystem()
        {
            InitializeComponent();

            WindowFormLoad();

            //  グラフィックライブラリ
            ydraw = new YWorldDraw(CvSoloarSystem);
            //  日時の設定
            mDateTime = DateTime.Now;
            //  コンボボックスの設定
            CbDispType.ItemsSource = mPlanetType;
            CbDispType.SelectedIndex = 0;
            CbStepDays.ItemsSource = mStepDayTutle;
            CbStepDays.SelectedIndex = 0;
            //  タイマーインスタンスの作成
            dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal);
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);

            //  地心軌道の軌跡データの初期化
            ptolemaicInit();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            drawSolarSystem(true, new PointD());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WindowFormSave();
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
            drawSolarSystem(true, new PointD());
        }

        /// <summary>
        /// Windowの状態を前回の状態にする
        /// </summary>
        private void WindowFormLoad()
        {
            //  前回のWindowの位置とサイズを復元する(登録項目をPropeties.settingsに登録して使用する)
            Properties.Settings.Default.Reload();

            if (Properties.Settings.Default.SolarSystemWidth < 100 || Properties.Settings.Default.SolarSystemHeight < 100 ||
                SystemParameters.WorkArea.Height < Properties.Settings.Default.SolarSystemHeight) {
                Properties.Settings.Default.SolarSystemWidth = mWindowWidth;
                Properties.Settings.Default.SolarSystemHeight = mWindowHeight;
            } else {
                Top = Properties.Settings.Default.SolarSystemTop;
                Left = Properties.Settings.Default.SolarSystemLeft;
                Width = Properties.Settings.Default.SolarSystemWidth;
                Height = Properties.Settings.Default.SolarSystemHeight;
                double dy = Height - mWindowHeight;
            }
        }

        /// <summary>
        /// Window状態を保存する
        /// </summary>
        private void WindowFormSave()
        {
            //  Windowの位置とサイズを保存(登録項目をPropeties.settingsに登録して使用する)
            Properties.Settings.Default.SolarSystemTop = Top;
            Properties.Settings.Default.SolarSystemLeft = Left;
            Properties.Settings.Default.SolarSystemWidth = Width;
            Properties.Settings.Default.SolarSystemHeight = Height;

            Properties.Settings.Default.Save();
        }


        /// <summary>
        /// 自動実行開始
        /// </summary>
        private void timerStart()
        {
            if (!dispatcherTimer.IsEnabled) {
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, mSecInterval, mMiliInterval);
                dispatcherTimer.Start();
            }
        }

        /// <summary>
        /// 自動実行停止
        /// </summary>
        private void timerStop()
        {
            if (dispatcherTimer.IsEnabled) {
                dispatcherTimer.Stop();
            }
        }

        /// <summary>
        /// タイマー割込みで惑星の位置を進める
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            mDateTime = mDateTime.AddDays(mDateIncrimentSize);
            DpSolarDate.SelectedDate = mDateTime;
            drawSolarSystem(false, new PointD());
        }

        /// <summary>
        /// [マウスホィール]
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CvSoloarSystem_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (0 != e.Delta) {
                PointD pos = screen2Canvas(new PointD(e.GetPosition(this)));
                mSolarSystemSize *= 1.0 - Math.Sign(e.Delta) * 0.1;
                drawSolarSystem(false, new PointD());
            }
        }

        /// <summary>
        /// [マウス移動]
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CvSoloarSystem_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //  スクリーン座標をCanvasの座標に
            PointD pos = screen2Canvas(new PointD(e.GetPosition(this)));
            if (!mLeftPressPoint.isEmpty() && e.LeftButton == System.Windows.Input.MouseButtonState.Pressed) {
                PointD dp = pos.vector(mLeftPressPoint);
                mRoollX -= dp.y / 10;
                mRoollY -= dp.x / 10;
                drawSolarSystem(false, new PointD());
                mLeftPressPoint = pos;
            }

        }

        /// <summary>
        /// [マウス左ボタンダウン]
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CvSoloarSystem_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            mLeftPressPoint = screen2Canvas(new PointD(e.GetPosition(this)));
        }

        /// <summary>
        /// [マウス左ボタンアップ]
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CvSoloarSystem_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            mLeftPressPoint.clear();
        }

        /// <summary>
        /// [表示タイプ]選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbDispType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbDispType.SelectedIndex == 1) {
                mPtolemaic = true;
            } else {
                mPtolemaic = false;
            }
            ptolemaicInit();
            drawSolarSystem(false, new PointD());
        }

        /// <summary>
        /// [ステップ日数] 設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbStepDays_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (0 < CbStepDays.SelectedIndex) {
                mDateIncrimentSize = ylib.string2int(mStepDayTutle[CbStepDays.SelectedIndex]);
            }
        }

        /// <summary>
        /// ボタン処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)e.Source;
            if (bt.Name.CompareTo("BtZoomUp") == 0) {               //  拡大
                mSolarSystemSize /= 1.1;
                drawSolarSystem(false, new PointD());
            } else if (bt.Name.CompareTo("BtZoomDown") == 0) {      //  縮小
                mSolarSystemSize *= 1.1;
                drawSolarSystem(false, new PointD());
            } else if (bt.Name.CompareTo("BtDateUp") == 0) {        //  進める
                mDateTime = mDateTime.AddDays(mDateIncrimentSize);
                DpSolarDate.SelectedDate = mDateTime;
                drawSolarSystem(false, new PointD());
            } else if (bt.Name.CompareTo("BtPlay") == 0) {          //  自動実行開始
                if (!dispatcherTimer.IsEnabled) {
                    timerStart();
                }
            } else if (bt.Name.CompareTo("BtDateNow") == 0) {       //  停止/現在時刻
                if (dispatcherTimer.IsEnabled) {
                    //  自動実行停止
                    timerStop();
                } else {
                    //  現在時刻の位置に設定
                    mDateTime = DateTime.Now;
                    DpSolarDate.SelectedDate = mDateTime;
                    ptolemaicInit();
                    drawSolarSystem(false, new PointD());
                }
            } else if (bt.Name.CompareTo("BtDateDown") == 0) {      //  戻す
                mDateTime = mDateTime.AddDays(-mDateIncrimentSize);
                DpSolarDate.SelectedDate = mDateTime;
                drawSolarSystem(false, new PointD());
            } else if (bt.Name.CompareTo("BtRollUp") == 0) {        //  X軸回転
                mRoollX += 5;
                drawSolarSystem(false, new PointD());
            } else if (bt.Name.CompareTo("BtRollReset") == 0) {     //  X軸回転Reset
                mRoollX = 0;
                mRoollY = 0;
                drawSolarSystem(false, new PointD());
            } else if (bt.Name.CompareTo("BtRollDown") == 0) {      //  X軸回転
                mRoollX += -5;
                drawSolarSystem(false, new PointD());
            } else if (bt.Name.CompareTo("BtExit") == 0) {          //  終了ボタン
                Close();
            }
        }

        /// <summary>
        /// カレンダ設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DpSolarDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mDateTime.CompareTo(DpSolarDate.SelectedDate.Value) != 0) {
                mDateTime = DpSolarDate.SelectedDate.Value;
                ptolemaicInit();
                drawSolarSystem(false, new PointD());
            }
        }

        /// <summary>
        /// 太陽系の描画
        /// </summary>
        /// <param name="init">初期化</param>
        /// <param name="cp">中心座標</param>
        /// <param name="zoom">ズーム</param>
        private void drawSolarSystem(bool init, PointD cp, double zoom = 1.0)
        {
            if (CvSoloarSystem.ActualWidth == 0 || CvSoloarSystem.ActualHeight == 0)
                return;

            windowSet(init, cp, zoom);
            drawBackGround();
            drawPlanet();
            drawLegend();
        }

        /// <summary>
        /// 描画領域の設定
        /// </summary>
        /// <param name="init">初期化</param>
        /// <param name="cp">中心座標</param>
        /// <param name="zoom">ズーム</param>
        private void windowSet(bool init, PointD cp, double zoom = 1.0)
        {
            //  Window設定
            if (!init && cp.isEmpty())
                cp = ydraw.mWorld.getCenter();

            string info = $"{mDateTime.Year}/{mDateTime.Month}/{mDateTime.Day}";
            info += $" 回転角 X軸 {mRoollX.ToString("0.##")} Y軸 {mRoollY.ToString("0.##")} 縮尺 {mSolarSystemSize.ToString("0.###")}";
            TbDate.Text = info;

            if (init ||
                (ydraw.mWorld.Width < ydraw.mWorld.Height ? ydraw.mWorld.Width * zoom > mSolarSystemSize * 1.2 * 2.0 :
                    ydraw.mWorld.Height * zoom > mSolarSystemSize * 1.2 * 2.0)) {
                ydraw.setViewArea(0.0, 0.0, CvSoloarSystem.ActualWidth, CvSoloarSystem.ActualHeight);
                ydraw.mAspectFix = true;
                ydraw.mClipping = true;
            }
            ydraw.setWorldWindow(-mSolarSystemSize * 1.1, mSolarSystemSize * 1.1, mSolarSystemSize * 1.1, -mSolarSystemSize * 1.1);
        }

        /// <summary>
        /// 太陽系の背景表示
        /// </summary>
        private void drawBackGround()
        {
            ydraw.clear();
            ydraw.matrixClear();
            ydraw.rotateX(ylib.D2R(mRoollX));
            ydraw.rotateY(ylib.D2R(mRoollY));

            ydraw.mFillColor = mBaseBackColor;
            ydraw.mBrush = Brushes.Black;
            Rect rect = new Rect(ydraw.mWorld.TopLeft.toPoint(), ydraw.mWorld.BottomRight.toPoint());
            ydraw.drawWRectangle(rect);

            ydraw.mBrush = Brushes.LightGray;
            ydraw.draw3DWLine(new Point3D(-mSolarSystemSize, 0, 0), new Point3D(mSolarSystemSize, 0, 0));
            ydraw.draw3DWLine(new Point3D(0, -mSolarSystemSize, 0), new Point3D(0, mSolarSystemSize, 0));

            //  中心太陽の表示
            ydraw.mBrush = mPtolemaic ? Brushes.Blue : Brushes.Red;
            ydraw.mFillColor = mPtolemaic ? Brushes.Blue : Brushes.Red;
            double r = ydraw.screen2worldXlength(7);
            ydraw.drawWCircle(new PointD(), r);
        }

        /// <summary>
        /// 惑星の位置表示
        /// </summary>
        private void drawPlanet()
        {
            //  ユリウス日
            double jd = ylib.getJD(mDateTime.Year, mDateTime.Month, mDateTime.Day);

            PLANETDATA earthData = plib.getPlanetData(mPlanetName[2]);

            for (int j = 0; j < mPlanetName.Length; j++) {
                //  惑星データ
                PLANETDATA pData = plib.getPlanetData(mPlanetName[j]);
                if (existPlanet(pData.a))
                    continue;
                //  表示属性設定
                ydraw.mBrush = Brushes.Blue;
                Point3D sp = pData.getPlanetPos(jd);
                if (mPtolemaic) {
                    if (j == 2) {
                        sp.inverse();
                    } else {
                        Point3D op = earthData.getPlanetPos(jd);
                        op.inverse();
                        sp.offset(op);
                    }
                }
                Point3D ssp = sp;
                ydraw.mBrush = Brushes.Black;
                //  惑星軌道表示
                double period = pData.periodDays();
                ydraw.mBrush = Brushes.Black;
                ydraw.mThickness = 1.0;
                for (int i = 0; i <= period; i += (int)(period / 48)) {
                    double jd2 = jd + i;
                    Point3D ep = pData.getPlanetPos(jd2);
                    if (mPtolemaic) {
                        if (j == 2) {
                            ep.inverse();
                        } else {
                            Point3D op = earthData.getPlanetPos(jd);
                            op.inverse();
                            ep.offset(op);
                        }
                    }
                    ydraw.draw3DWLine(sp, ep);
                    sp = ep;
                }
                ydraw.draw3DWLine(sp, ssp);

                //  惑星位置表示
                ydraw.mFillColor = (mPtolemaic && j == 2) ? Brushes.Red : mPlanetColor[j];
                double r = ydraw.screen2worldXlength(4);
                ydraw.draw3DWCircle(ssp, r);

                mPtlemaicList[j].Add(ssp);      //  軌跡データの登録

                //  惑星の軌跡の表示
                if (mPtolemaic && 1< mPtlemaicList[j].Count) {
                    ydraw.mBrush = (mPtolemaic && j == 2) ? Brushes.Red : mPlanetColor[j];
                    ydraw.mThickness = 2.0;
                    for (int i = 0; i < mPtlemaicList[j].Count - 1; i++) {
                        ydraw.draw3DWLine(mPtlemaicList[j][i], mPtlemaicList[j][i + 1]);
                    }
                }
            }
        }

        /// <summary>
        /// 地球中心の惑星の軌跡データの初期化
        /// </summary>
        private void ptolemaicInit()
        {
            mPtlemaicList = new List<List<Point3D>>();
            for (int j = 0; j < mPlanetName.Length; j++) {
                List<Point3D> buf = new List<Point3D>();
                mPtlemaicList.Add(buf);
            }
        }

        /// <summary>
        /// 凡例表示
        /// </summary>
        private void drawLegend()
        {
            ydraw.mTextSize = 15;
            double x = 20;
            double y = 20;
            ydraw.mFillColor = Brushes.Red;
            PointD p = new PointD(x, y);
            ydraw.drawCircle(p, 5);
            ydraw.mBrush = Brushes.Black;
            p.Offset(15, -10);
            ydraw.drawText("太陽", p.toPoint());
            y += 20;
            for (int i = 0; i < mPlanetColor.Length; i++) {
                PLANETDATA pData = plib.getPlanetData(mPlanetName[i]);
                if (existPlanet(pData.a))
                    continue;
                p = new PointD(x,y);
                ydraw.mFillColor = mPlanetColor[i];
                ydraw.drawCircle(p, 5);
                ydraw.mBrush = Brushes.Black;
                p.Offset(15, -10);
                ydraw.drawText(mPlanetName[i], p.toPoint());
                y += 20;
            }
        }

        /// <summary>
        /// 非表示判定
        /// </summary>
        /// <param name="a">惑星の軌道半径</param>
        /// <returns>非表示</returns>
        private bool existPlanet(double a)
        {
            if (mSolarSystemSize * 1.5 < a || a < mSolarSystemSize * 0.03)
                return true;
            else
                return false;
        }

        /// <summary>
        /// スクリーン座標からCanvasの座標に変換
        /// 左と上のコントロール分オフセットする
        /// </summary>
        /// <param name="sp">スクリーン座標</param>
        /// <returns>Cnavas座標</returns>
        private PointD screen2Canvas(PointD sp)
        {
            PointD offset = new PointD(0.0, SbTopStatusBar.ActualHeight);
            sp.Offset(-offset.x, -offset.y);
            return sp;
        }
    }
}
