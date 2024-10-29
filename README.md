# WordCloud Application

## Overview

The WordCloud application is a web-based tool designed to create dynamic word clouds. Users can input words, which will then be visualized with varying sizes based on frequency. This project utilizes the ASP.NET Core framework and supports cross-platform deployment, including Linux-based environments.

## Features

- **Dynamic Word Cloud Generation**: Add words to the cloud dynamically, with the size of each word changing based on the number of times it has been added.
- **Real-time WebSockets**: Real-time updates using SignalR to allow users to see word additions instantly.
- **Database Integration**: Uses Entity Framework Core for SQL Server, enabling persistent word storage and retrieval.
- **Cross-Platform Support**: Built using .NET 8, with support for self-contained deployment on Linux (including `libSkiaSharp.so` integration).
- **Application Insights**: Integrated with Microsoft Application Insights for monitoring and performance tracking.

## Prerequisites

- .NET 8 SDK
- Visual Studio Code, JetBrains Rider, or Visual Studio
- SQL Server or another compatible database
- Linux server for deployment (if deploying cross-platform)
- Application Insights (optional, for monitoring)

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd WordCloud
