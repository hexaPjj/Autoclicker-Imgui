using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;
using ClickableTransparentOverlay;
using ImGuiNET;

namespace imgui_autooclicker
{
    public class Renderer : Overlay
    {
        #region Win32 API

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, IntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKey);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        #endregion

        #region Constants

        const int SW_HIDE = 0;

        const uint LEFTDOWN = 0x02;
        const uint LEFTUP = 0x04;
        const int HOTKEY = 0x46; // "F" tuşu

        const uint RIGHTDOWN = 0x08;
        const uint RIGHTUP = 0x10;
        const int RHOTKEY = 0x56;
        #endregion

        #region Variables

        int cpslidervalue = 13;
        int rightcpsslidervalue = 20;

        bool isleftAutocheckbox = false;
        bool isrightAutocheckbox = false;

        Vector4 selectedColor = new Vector4(.261f,.876f,.601f,1f);

        bool clickEnabled = false;
        bool rightclickEnabled = false;
        float time = 0.0f;

        Thread clickThread;
        Thread rightclickThread;

        #endregion

        #region console + click
        public Renderer()
        {
            IntPtr handle = GetConsoleWindow();
            if (handle != IntPtr.Zero)
            {
                ShowWindow(handle, SW_HIDE);
            }

            // Yeni bir thread başlat ve tıklamayı kontrol et
            clickThread = new Thread(AutoClickLoop);
            clickThread.IsBackground = true;
            clickThread.Start();

            rightclickThread = new Thread(rightAutoClickLoop); 
            rightclickThread.IsBackground = true;
            rightclickThread.Start();
        }

        #region leftclick
        async void AutoClickLoop()
        {
            Stopwatch stopwatch = new Stopwatch();
            while (true)
            {
                if (GetAsyncKeyState(HOTKEY) < 0 && isleftAutocheckbox)
                {
                    clickEnabled = !clickEnabled;
                    await Task.Delay(200);
                }

                if (clickEnabled)
                {
                    stopwatch.Restart();
                    leftmouseClick();

                    double clickInterval = 1000.0 / cpslidervalue;
                    while (stopwatch.Elapsed.TotalMilliseconds < clickInterval)
                    {
                        await Task.Delay((int)0.3); // Daha hassas zamanlama
                    }
                }
                else
                {
                    await Task.Delay((int)0.1);
                }
            }
        }
        void leftmouseClick()
        {
            mouse_event(LEFTDOWN, 0, 0, 0, IntPtr.Zero);
            mouse_event(LEFTUP, 0, 0, 0, IntPtr.Zero);
        }

        #endregion

        #region rightclick

        async void rightAutoClickLoop()
        {
            Stopwatch stopwatch = new Stopwatch();
            while (true)
            {
                if (GetAsyncKeyState(RHOTKEY) < 0 && isrightAutocheckbox)
                {
                    rightclickEnabled = !rightclickEnabled;
                    await Task.Delay(200);
                }

                if (rightclickEnabled)
                {
                    stopwatch.Restart();
                    rightmouseClick();

                    double clickInterval = 1000.0 / rightcpsslidervalue;
                    while (stopwatch.Elapsed.TotalMilliseconds < clickInterval)
                    {
                        await Task.Delay((int)0.3); // Daha hassas zamanlama
                    }
                }
                else
                {
                    await Task.Delay((int)0.1);
                }
            }
        }


        void rightmouseClick()
        {
            mouse_event(RIGHTDOWN, 0, 0, 0, IntPtr.Zero);
            mouse_event(RIGHTUP, 0, 0, 0, IntPtr.Zero);
        }


        #endregion

        #endregion

        #region 1.PENCERE
        protected override void Render()
        {
            //===========================BEGIN=================================


            ReplaceFont("C:\\Windows\\Fonts\\arial.ttf", 15,FontGlyphRangeType.English);
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.0f, 0.0f, 0.0f, 1.0f)); // Siyah renk

            ImGui.Begin("SPLUS CLICKER");
            //ImGui.ShowStyleEditor();
            ImGui.SetNextWindowSize(new Vector2(600, 700), ImGuiCond.FirstUseEver);

            ImGui.PopStyleColor();

            //=================================================================

            #region style
            var style = ImGui.GetStyle();
            var colors = style.Colors;

            style.WindowRounding = 10f;
            style.FrameRounding = 6f;
            style.GrabRounding = 4f;
            style.FrameBorderSize = 1f;
            style.WindowBorderSize = 1f;
            style.ItemSpacing = new Vector2(10, 8);
            style.ScrollbarSize = 12f;

            style.WindowMenuButtonPosition = ImGuiDir.Right;
            style.WindowTitleAlign = new Vector2(0.50f, 0.50f);

            //text
            colors[(int)ImGuiCol.Text] = new Vector4(0.95f, 0.96f, 0.98f, 1.00f);

            //window
            colors[(int)ImGuiCol.WindowBg] = new Vector4(0.08f, 0.08f, 0.10f, 1.00f);
            colors[(int)ImGuiCol.ChildBg] = new Vector4(0.10f, 0.10f, 0.12f, 1.00f);

            //checkbox
            colors[(int)ImGuiCol.FrameBg] = new Vector4(0.12f, 0.12f, 0.15f, 1.00f); // checkbox nonhovered color
            colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.20f, 0.20f, 0.25f, 1.00f); //hover
            colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.30f, 0.30f, 0.35f, 1.00f); //active
            style.Colors[(int)ImGuiCol.CheckMark] = new System.Numerics.Vector4(0.0f, 0.48f, 1.0f, 1.0f); //check color blue

            //title
            colors[(int)ImGuiCol.TitleBg] = new Vector4(0.08f, 0.08f, 0.10f, 1.00f);
            colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.10f, 0.10f, 0.12f, 1.00f);

            //button
            colors[(int)ImGuiCol.Button] = new Vector4(0.18f, 0.18f, 0.22f, 1.00f);
            colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.28f, 0.28f, 0.35f, 1.00f);
            colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.40f, 0.40f, 0.50f, 1.00f);


            #endregion

            #region tab

            if (ImGui.BeginTabBar("Tabs"))
            {

                ImGui.PushStyleColor(ImGuiCol.Tab, new Vector4(0.2f, 0.2f, 0.2f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.TabHovered, new Vector4(0.4f, 0.4f, 0.4f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.TabActive, new Vector4(0.4f, 0.4f, 0.4f, 1.0f));



                ImGui.GetStyle().Colors[(int)ImGuiCol.Border] = selectedColor;
                ImGui.GetStyle().Colors[(int)ImGuiCol.TitleBgActive] = selectedColor;

                if (ImGui.BeginTabItem("LeftClick"))
                {
                    ImGui.Text("Key: F");
                    ImGui.SliderInt("CPS", ref cpslidervalue, 8, 100);
                    ImGui.Spacing();
                    ImGui.Spacing();

                    ImGui.SeparatorText("ON - OFF");
                    ImGui.Spacing();

                    ImGui.Text(clickEnabled ? "Clicking: ON" : "Clicking: OFF");

                    ImGui.Text(isleftAutocheckbox ? "Active: ON" : "Active: OFF");


                    ImGui.EndTabItem();
                }


                if (ImGui.BeginTabItem("RightClick"))
                {
                    ImGui.Text("Key: V");
                    ImGui.SliderInt("CPS", ref rightcpsslidervalue, 14, 140);
                    ImGui.Spacing();
                    ImGui.Spacing();

                    ImGui.SeparatorText("ON - OFF");
                    ImGui.Spacing();
                    ImGui.Text(clickEnabled ? "Clicking: ON" : "Clicking: OFF");
                    ImGui.Text(isrightAutocheckbox ? "Active: ON" : "Active: OFF");


                    ImGui.EndTabItem();
                }



                if (ImGui.BeginTabItem("Misc"))
                {

                    if (isleftAutocheckbox)
                    {
                        ImGui.PushStyleColor(ImGuiCol.CheckMark, selectedColor);
                    }

                    if (ImGui.Checkbox("Active Left Click", ref isleftAutocheckbox)){ }
                    ImGui.Spacing();

                    if (isrightAutocheckbox)
                    {
                        ImGui.PushStyleColor(ImGuiCol.CheckMark, selectedColor);
                    }

                    if (ImGui.Checkbox("Active Right Click", ref isrightAutocheckbox)) { }
                    ImGui.Spacing();

                    ImGui.Spacing();
                    ImGui.SeparatorText("Custom Color"); 
                    ImGui.Spacing();

                    if (ImGui.ColorButton("Color Change", selectedColor))
                    {
                        ImGui.OpenPopup("ColorPicker");
                    }

                    ImGui.SameLine();
                    ImGui.Text("Color Change");

                    if (ImGui.BeginPopup("ColorPicker"))
                    {
                        ImGui.ColorPicker4("##picker", ref selectedColor, ImGuiColorEditFlags.NoSidePreview | ImGuiColorEditFlags.DisplayRGB);
                        ImGui.EndPopup();
                    }



                    ImGui.EndTabItem();
                }


                ImGui.EndTabBar();
            }

            #endregion


            ImGui.End();

        }
        #endregion


    }
}
