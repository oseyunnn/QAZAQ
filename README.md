# QAZAQ

QAZAQ is a modern Kazakhstan tourism website designed to present the country as a premium travel destination through elegant editorial storytelling, curated destination content, and a streamlined booking experience.

The platform combines destination discovery, cultural heritage highlights, traveler reviews, and trip planning in one cohesive digital experience tailored to users exploring Kazakhstan for leisure, cultural immersion, and adventure.

## Overview

QAZAQ is built to celebrate Kazakhstan’s natural beauty, historic significance, and unique travel appeal. The website showcases:

- curated destinations and travel highlights
- cultural heritage and regional storytelling
- traveler reviews and social trust signals
- a booking journey for trip planning and reservation requests
- a polished, premium visual identity inspired by luxury editorial travel brands

## Objectives

The project aims to:

- present Kazakhstan as a destination of depth and diversity
- provide an engaging travel discovery experience for visitors
- support trip planning through a clear booking flow
- create a maintainable, scalable frontend architecture for future growth

## Tech Stack

- ASP.NET Core
- Blazor Web App on .NET 8
- Razor Components
- Interactive Server rendering
- Tailwind CSS v4 for styling and layout
- C# models and sample data for travel content and booking forms

## Project Structure

- Program.cs — ASP.NET Core startup and Blazor registration
- QAZAQ.csproj — .NET project configuration and front-end build setup
- Components/ — application layout, routing, and page components
- Models/ — domain entities, enums, and form models
- Data/SampleData.cs — static tourism, review, and heritage content
- Styles/app.css — global theme tokens and shared styling
- wwwroot/ — static assets and frontend scripts
- appsettings.json and appsettings.Development.json — environment configuration

## Features

- luxury travel landing page experience
- destination discovery and feature sections
- cultural heritage storytelling
- traveler review showcase
- booking form for trip inquiry and reservation planning
- responsive layout for desktop and mobile viewing

## Prerequisites

Before running the application, make sure you have the following installed:

- .NET 8 SDK
- Node.js 18+ and npm
- A local terminal such as PowerShell, Command Prompt, or Bash

## Run the Project Locally

1. Install the frontend dependencies:

   npm install

2. Build the CSS bundle:

   npm run build:css

   If you want automatic rebuilds while developing, you can run:

   npm run watch:css

3. Restore the .NET dependencies:

   dotnet restore

4. Start the application:

   dotnet run

5. Open the local URL displayed in the terminal in your browser.

If you prefer live development updates during editing, you can also run:

   dotnet watch

## Recommended Development Flow

To keep the project organized as it evolves, use this order when editing:

1. Styles/app.css — visual theme, typography, spacing, and layout system
2. Models/ — content models and form structures
3. Data/SampleData.cs — destination and review data
4. Components/Layout/ — shared navigation and page shell
5. Components/Pages/ — page content and user-facing sections

## Notes

This project is intentionally structured to separate visual design, data, and business logic for easier long-term maintenance and clearer future expansion. It is well suited for additional destinations, booking enhancements, destination filtering, and content-driven travel features.

## License

This project is intended for portfolio and demonstration purposes.

