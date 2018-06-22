//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SDKTemplate
{
    public struct IntPoint {
        public int X;
        public int Y;
    }

    public sealed partial class Scenario3_ServerForeground : Page
    {
        private MainPage rootPage = MainPage.Current;

        private eBeamSDKLib sdkLib;

        private int m_calStatus = 0;
        ///Actually User don't click the exact edge of paper. 
        ///the margin between the edge of pager and click point.
        /// 
        private const int CALIBRATION_MARGIN = 100; // about 8mm

        private IntPoint[] m_CalPoint = new IntPoint[4];

        #region UI Code
        public Scenario3_ServerForeground()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("OnNavigatedTo Enter Calibration Page");


            sdkLib = rootPage.GetEBeam();
            if(sdkLib != null)
            {
                sdkLib.setCalirationMode(true);
                if(sdkLib.getStationPosition() == eBeamSDKLib.DIRECTION_LEFT)
                {
                    LeftSide.IsChecked = true;
                }
                else
                {
                    RightSide.IsChecked = true;
                }
                sdkLib.firePenDataEvent += OnPenDataHandlerAsync;

                
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("OnNavigatedFrom Exit Calibration Page");
            if (sdkLib != null)
            {
                sdkLib.firePenDataEvent -= OnPenDataHandlerAsync;
                sdkLib.setCalirationMode(false);
            }

        }

        #endregion

        public async void OnPenDataHandlerAsync(object sender, PenDataEvent e)
        {
 

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                  () =>
                  {
                      
                      switch (e.pen_rec.PenStatus)
                      {
                          case eBeamSDKLib.PEN_DOWN:
                             
                              break;
                          case eBeamSDKLib.PEN_MOVE:
                               
                              break;
                          case eBeamSDKLib.PEN_UP:
                              m_calStatus++;
                              switch (m_calStatus)
                              {
                                  case 0: /// 
                                      break;
                                  case 1: ///
                                      calNumber1Do.Visibility = Visibility.Collapsed;
                                      calNumber1Check.Visibility = Visibility.Visible;
                                      calNumber1Text.Visibility = Visibility.Collapsed;


                                      calNumber2ColorPre.Visibility = Visibility.Collapsed;
                                      calNumber2ColorDo.Visibility = Visibility.Visible;
                                      calNumber2Do.Visibility = Visibility.Visible;
                                      calNumber2Check.Visibility = Visibility.Collapsed;
                                      calNumber2Text.Visibility = Visibility.Visible;
                                      if (LeftSide.IsChecked == true)
                                      {
                                          m_CalPoint[0].X = e.pen_rec.X - CALIBRATION_MARGIN;
                                          m_CalPoint[0].Y = e.pen_rec.Y - CALIBRATION_MARGIN;

                                      }
                                      else
                                      {
                                          m_CalPoint[3].X = e.pen_rec.X + CALIBRATION_MARGIN;
                                          m_CalPoint[3].Y = e.pen_rec.Y - CALIBRATION_MARGIN;

                                      }

                                      break;
                                  case 2:
                                      calNumber2ColorPre.Visibility = Visibility.Collapsed;
                                      calNumber2ColorDo.Visibility = Visibility.Visible;
                                      calNumber2Do.Visibility = Visibility.Collapsed;
                                      calNumber2Check.Visibility = Visibility.Visible;
                                      calNumber2Text.Visibility = Visibility.Collapsed;

                                      calNumber2Text.Text = "Calibration is Finished.";
                                      if (LeftSide.IsChecked == true)
                                      {
                                          m_CalPoint[2].X = e.pen_rec.X + CALIBRATION_MARGIN;
                                          m_CalPoint[2].Y = e.pen_rec.Y + CALIBRATION_MARGIN;
                                          m_CalPoint[1].X = m_CalPoint[0].X;
                                          m_CalPoint[1].Y = m_CalPoint[2].Y;
                                          m_CalPoint[3].X = m_CalPoint[2].X;
                                          m_CalPoint[3].Y = m_CalPoint[0].Y;

                                      }
                                      else
                                      {
                                          m_CalPoint[1].X = e.pen_rec.X - CALIBRATION_MARGIN;
                                          m_CalPoint[1].Y = e.pen_rec.Y + CALIBRATION_MARGIN;
                                          m_CalPoint[0].X = m_CalPoint[1].X;
                                          m_CalPoint[0].Y = m_CalPoint[3].Y;
                                          m_CalPoint[2].X = m_CalPoint[3].X;
                                          m_CalPoint[2].Y = m_CalPoint[1].Y;

                                      }

                                      int pos = (LeftSide.IsChecked == true) ? 
                                                eBeamSDKLib.DIRECTION_LEFT : eBeamSDKLib.DIRECTION_RIGHT;
                                      sdkLib.setSMPosition(pos);
                                      sdkLib.SaveCalibrationToDevice(m_CalPoint[0].X, m_CalPoint[0].Y,
                                          m_CalPoint[1].X, m_CalPoint[1].Y,
                                          m_CalPoint[2].X, m_CalPoint[2].Y,
                                          m_CalPoint[3].X, m_CalPoint[3].Y);
                                     
                                      break;
                              }

                              break;
                          case eBeamSDKLib.PEN_HOVER:
                              break;

                      }

                  });


        }
        public async void OnPenConditionHandlerAsync(object sender, PenConditionEvent e)
        {
            var newValue = string.Format("Sensor Position={0} \r\n", e.conditionData.StationPosition);
            newValue += string.Format("Battery Sensor={0}(%) Pen={1} (100:High Others:Low) \r\n", e.conditionData.battery_station, e.conditionData.battery_pen);

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                  () =>
                  {
                       
                  });


        }
        public async void OnPenConnectionHandlerAsync(object sender, PenConnectionEvent e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                  () =>
                  {
                      switch (e.status)
                      {
                          case eBeamSDKLib.PEN_BLE_CONN:
 
                              break;
                          case eBeamSDKLib.PEN_DATA_READY:
                              
                              break;
                          case eBeamSDKLib.PEN_DISCONNECT:
                              
                              break;
                          case eBeamSDKLib.PEN_DATA_STOP:
                              
                              break;
                      }

                  });


        }
        public async void OnPenButtonHandlerAsync(object sender, PenButtonEvent e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                  () =>
                  {
                       
                  });


        }
        private void onStationSide_Checked()
        {
            System.Diagnostics.Debug.WriteLine("onStationSide_Checked");
            if (LeftSide.IsChecked == true)
            {
               if(sensorImg != null)  sensorImg.SetValue(Canvas.LeftProperty, 0);
                if (backImg != null) backImg.SetValue(Canvas.LeftProperty, 100);
                if (deadZoneRect != null) deadZoneRect.SetValue(Canvas.LeftProperty, 70 );
                if (deadZoneText != null) deadZoneText.SetValue(Canvas.LeftProperty, 70);
                if (calNumber1Do != null) calNumber1Do.SetValue(Canvas.LeftProperty, 110);
                if (calNumber1Check != null) calNumber1Check.SetValue(Canvas.LeftProperty, 110);
                if (calNumber1ColorPre != null) calNumber1ColorPre.SetValue(Canvas.LeftProperty, 105);
                if (calNumber1Text != null) calNumber1Text.SetValue(Canvas.LeftProperty, 140);
                if (calNumber2Do != null) calNumber2Do.SetValue(Canvas.LeftProperty, 770);
                if (calNumber2ColorDo != null) calNumber2ColorDo.SetValue(Canvas.LeftProperty, 770);
                if (calNumber2ColorPre != null) calNumber2ColorPre.SetValue(Canvas.LeftProperty, 770);
                if (calNumber2Check != null) calNumber2Check.SetValue(Canvas.LeftProperty, 770);
                if (calNumber2Text != null) calNumber2Text.SetValue(Canvas.LeftProperty, 770);

            }
            else
            {
                if (sensorImg != null) sensorImg.SetValue(Canvas.LeftProperty, 800-70);
                if (backImg != null) backImg.SetValue(Canvas.LeftProperty,0);
                if (deadZoneRect != null) deadZoneRect.SetValue(Canvas.LeftProperty, 800 - 70 - 30);
                if (deadZoneText != null) deadZoneText.SetValue(Canvas.LeftProperty, 800 - 70 - 30);

                if (calNumber1Do != null) calNumber1Do.SetValue(Canvas.LeftProperty, 110-100);
                if (calNumber1Check != null) calNumber1Check.SetValue(Canvas.LeftProperty, 110 -100);
                if (calNumber1ColorPre != null) calNumber1ColorPre.SetValue(Canvas.LeftProperty, 105-100);
                if (calNumber1Text != null) calNumber1Text.SetValue(Canvas.LeftProperty, 140 - 100);

                if (calNumber2Do != null) calNumber2Do.SetValue(Canvas.LeftProperty, 770 - 100);
                if (calNumber2ColorDo != null) calNumber2ColorDo.SetValue(Canvas.LeftProperty, 770 - 100);
                if (calNumber2ColorPre != null) calNumber2ColorPre.SetValue(Canvas.LeftProperty, 770 - 100);
                if (calNumber2Check != null) calNumber2Check.SetValue(Canvas.LeftProperty, 770 - 100);
                if (calNumber2Text != null) calNumber2Text.SetValue(Canvas.LeftProperty, 770 - 100);
            }
        }
    }
}
