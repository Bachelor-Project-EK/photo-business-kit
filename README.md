# photo-business-kit

Bachelor project: A photography booking system where event requests await admin approval. The solution includes a structured portfolio, album-based photo presentation, backend API endpoints, and backoffice configuration through Umbraco. It is designed to streamline workflow, organize visual content, and support efficient management for photography businesses.

## Prerequisites

Before running the project, make sure the following tools are installed:

* .NET SDK
* Docker Desktop
* Aspire CLI

Docker Desktop must be running before starting the application, because the project uses Aspire to run container-based resources locally.

## Install Aspire CLI

### Windows PowerShell

```powershell
irm https://aspire.dev/install.ps1 | iex
```

### macOS/Linux

```bash
curl -sSL https://aspire.dev/install.sh | bash
```

After installation, verify that Aspire CLI is available:

```bash
aspire --version
```

If the command is not recognized, close and reopen the terminal, then try again.

## How to run the project

### 1. Start the application

Navigate to the repository root and run the project with Aspire:

```bash
aspire run
```

Aspire will start the AppHost, required services, container resources, and the Aspire dashboard.

After the application has started, open the URL shown in the Aspire dashboard.

### 2. Create the Umbraco backoffice user

When Umbraco starts for the first time, you will be asked to create a backoffice user. Complete the setup and log in to the Umbraco backoffice.

The Umbraco backoffice can be accessed at:

```text
https://localhost:<port>/umbraco
```

### 3. Import demo configuration with uSync

Before using the demo frontend, the Umbraco configuration must be imported.

In the Umbraco backoffice, go to:

```text
Settings → uSync → Import Everything
```

This imports the required demo frontend configuration and content setup.

### 4. Open the demo frontend

After importing the uSync configuration, the demo frontend can be accessed from the root URL:

```text
https://localhost:<port>/
```

Make sure uSync has been imported before opening the frontend. Otherwise, the demo content and configuration may not be available.

### 5. Open Swagger API documentation

The Swagger endpoint can be accessed at:

```text
https://localhost:<port>/umbraco/swagger
```

To use the secured endpoints, click **Authorize** in Swagger. You will be redirected to log in with the Umbraco backoffice user. After login, Swagger will be authorized and the protected API endpoints can be tested.
