# smERP Frontend

This is the frontend application for smERP, a small Enterprise Resource Planning (ERP) system. It is built using React 18, TypeScript, and Material-UI 5.

## Project Overview

This frontend application complements the smERP backend, providing a user-friendly interface for interacting with the ERP system.
I used React instead of Angular which is commonly used with .NET to familiars myself with react

## Technologies Used

- React 18
- TypeScript
- Material-UI 5
- React Query
- React Router
- React Hook Form

## Key Features

- Responsive and modern user interface using Material-UI components
- Efficient state management and server-state synchronization with React Query
- Form handling and validation using React Hook Form
- Theming and styling with Emotion

## Project Structure

The project follows a modular structure, with reusable components and utility functions organized into appropriate directories. Key folders include:

- `src/components`: Reusable UI components
- `src/pages`: Main application pages
- `src/hooks`: Custom React hooks
- `src/utils`: Utility functions and helpers
- `src/theme`: Material-UI theme configuration
- `src/api`: API integration and data fetching logic

## Getting Started

1. Clone the repository
2. Install dependencies:
   ```
   npm install
   ```
3. Start the development server:
   ```
   npm start
   ```

## Available Scripts

- `npm start`: Runs the app in development mode
- `npm test`: Launches the test runner
- `npm run build`: Builds the app for production
- `npm run eject`: Ejects from Create React App (use with caution)

## Main Dependencies

```json
{
  "dependencies": {
    "@emotion/react": "^11.13.3",
    "@emotion/styled": "^11.13.0",
    "@mui/material": "^5.16.7",
    "@mui/lab": "^5.0.0-alpha.173",
    "@mui/x-date-pickers": "^6.18.1",
    "@tanstack/react-query": "^5.59.0",
    "react": "^18.3.1",
    "react-dom": "^18.3.1",
    "react-router-dom": "^6.26.1",
    "react-hook-form": "^7.53.0",
    "apexcharts": "^3.52.0",
    "react-apexcharts": "^1.4.1"
  }
}
```

## Generic Components

The project utilizes a set of generic components to maintain consistency and reduce code duplication. These components include:

- Text form fields
- Select inputs
- Telephone inputs
- Date pickers
- Tables

These components are designed to fit most use cases within the project and can be easily customized or extended as needed.

## Contact

For any questions or feedback regarding the frontend application, please contact:

Omar Metwally - xomar.metwallyx@gmail.com

---

This frontend project is actively under development. Stay tuned for updates and new features!