# Live Optics Report Generator

A Windows WPF application designed to ingest Dell Live Optics assessment data (`.xlsx`), visualize key performance metrics, and act as an AI-powered research assistant for IT infrastructure analysis.

![Status](https://img.shields.io/badge/status-active-success.svg)
![License](https://img.shields.io/badge/license-BSD--3--Clause-blue.svg)

## 🚀 Features

*   **Data Visualization**: Instantly graph IOPS and Throughput trends from Live Optics Excel exports.
*   **AI Research Agent**: A built-in "Research Agent" sidebar that analyzes your server/disk metrics and provides insights (simulated for demo).
*   **Report Generation**: Automatically generates a PowerPoint (`.pptx`) presentation summarizing the project, including executive summaries and hardware stats.
*   **Modern UI**: Clean, MVVM-based WPF interface using `LiveCharts2` for high-performance rendering.

## 📋 Prerequisites

To build and run this project, you need:

*   **operating System**: Windows 10/11 (Required for running the WPF GUI).
*   **.NET 8 SDK**: [Download here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).
*   **Node.js** (Optional, for building the installer): [Download here](https://nodejs.org/).

## 🛠️ Installation & Setup

1.  **Clone the Repository**
    ```bash
    git clone https://github.com/TheArchitectit/radreportgenerator.git
    cd radreportgenerator/ReportGenerator
    ```

2.  **Open in Visual Studio**
    *   Open `LiveOptics.sln`.
    *   Build the Solution (`Ctrl+Shift+B`).
    *   Run (`F5`).

## 📦 Building the Application

 You can build the application in two ways: as a portable executable or as a full Windows Installer.

### Option 1: Portable Executable (No Install Required)
This creates a single `.exe` file that includes all dependencies (even the .NET runtime).

**Using the Batch Script:**
Double-click `publish_portable.cmd` in the root folder. The output will be in the `PortableBuild` folder.

**Using Command Line:**
```bash
npm run build:dotnet
```

### Option 2: Windows Installer (Setup.exe)
This requires `npm` to be installed. It uses Inno Setup to create a professional installer.

1.  **Install Dependencies**
    ```bash
    npm install
    ```

2.  **Build Installer**
    ```bash
    npm run dist
    ```
    *   The final installer will be located in the `Installer` folder (e.g., `Installer/LiveOpticsSetup.exe`).

## 🎮 Usage Guide

1.  **Launch the App**: Run `LiveOptics.UI.Wpf.exe` (or use the installed shortcut).
2.  **Load Data**: Click **Load .xlsx** and select your Live Optics export file.
3.  **Analyze**:
    *   Review the **Dashboard** for server counts and performance graphs.
    *   Check the **AI Sidebar** for automatic insights on high-latency disks or legacy hardware.
    *   Click **Run Analysis Agent** to trigger a deeper scan of the loaded metrics.
4.  **Export**: Click **Generate Report** to save a PowerPoint summary of your findings.

## 🏗️ Project Structure

*   `src/LiveOptics.Core`: Shared logic, data models, and parsers (Excel/OpenXML).
*   `src/LiveOptics.UI.Wpf`: Main Windows application (XAML, ViewModels).
*   `src/LiveOptics.Tests`: Unit tests for verifying logic.

## 🤝 Contributing

1.  Fork the repository.
2.  Create your feature branch (`git checkout -b feature/AmazingFeature`).
3.  Commit your changes (`git commit -m 'Add some AmazingFeature'`).
4.  Push to the branch (`git push origin feature/AmazingFeature`).
5.  Open a Pull Request.

## 📄 License

Distributed under the BSD-3-Clause License. See `LICENSE` for more information.