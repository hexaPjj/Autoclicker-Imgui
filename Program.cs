using imgui_autooclicker;
using ImGuiNET;
using ClickableTransparentOverlay;

Renderer renderer = new Renderer();
Thread rendererThread = new Thread(renderer.Start().Wait);
rendererThread.Start();