# Diabetes Prediction Project

## Table of Contents
- [Project Overview](#project-overview)
- [Project Objectives](#project-objectives)
- [Key Features](#key-features)
- [Project Structure](#project-structure)
- [Deliverables](#deliverables)
- [Technical Details](#technical-details)
    - [Technologies Used](#technologies-used)
    - [System Architecture](#system-architecture)
    - [Sprint 1](#sprint-1)
        - [Tasks](#sprint-1-tasks)
        - [REST Service](#sprint-1-rest-service)
        - [Docker Infrastructure](#sprint-1-docker-infrastructure)
    - [Sprint 2](#sprint-2)
        - [Tasks](#sprint-2-tasks)
        - [Report Generation](#sprint-2-report-generation)
    - [Sprint 3](#sprint-3)
        - [Tasks](#sprint-3-tasks)
        - [Integration](#sprint-3-integration)
- [Conclusion](#conclusion)

## Project Overview
The Diabetes Prediction Project is a comprehensive initiative to develop a diabetes prediction system for our client. This system leverages microservices and a RESTful architecture to facilitate diabetes prediction based on patient data and medical history.

## Project Objectives
The primary objectives of this project are as follows:
- Design and implement a robust Docker infrastructure for microservices.
- Create a RESTful service for accessing patient records.
- Develop a system for generating diabetes reports.
- Utilize .NET Core MVC as the primary framework for development.

## Key Features
1. **Docker Infrastructure**: The project involves setting up an efficient Docker infrastructure that allows for easy deployment and scaling of microservices.
2. **RESTful Service**: We create a RESTful API for seamless access to patient records, ensuring data availability for analysis.
3. **Diabetes Report Generation**: At the heart of the project is the ability to generate diabetes reports, aiding in medical decision-making.

## Project Structure
The project is organized into three sprints, each serving a specific purpose:

- **Sprint 1**: Focuses on laying the foundation for the project, including Docker infrastructure setup and the creation of a REST service.
- **Sprint 2**: Concentrates on additional feature development, especially the mechanism for generating diabetes reports.
- **Sprint 3**: Wraps up the project by integrating all microservices and finalizing the diabetes prediction system.

## Deliverables
The deliverables for this project are crucial to tracking progress and ensuring that the objectives are met. They include:
1. **Kanban Board**: A link to the updated Kanban board, which provides a visual representation of project tasks and progress.
2. **Retropective Templates**: Completed retrospective templates for each sprint, capturing insights and lessons learned.
3. **Code Project Archive**: A Zip archive of the project's code, including a link to the GitHub repository where the code is hosted. This also includes a test report and Docker configuration files.
4. **REST Service Documentation**: Comprehensive documentation outlining the REST services created, including API endpoints, request/response formats, and data models : 
    - <a href="../MediScreenApp/MediScreenApi/swagger/v1/swagger.pdf">PDF Swagger Documentation</a>
    - <a href="../MediScreenApp/MediScreenApi/swagger/v1/swagger.json">JSON Swagger Documentation</a>

## Technical Details
### Technologies Used
- **.NET Core MVC**: Chosen as the core framework for its versatility and compatibility with microservices.
- **Docker**: Facilitating containerization and deployment.
- **RESTful API**: Enabling data access and interaction.
- **Swagger**: Used for API documentation.
- **SQL Database**: Utilized for patient data storage.
- **MongoDb Database**: Utilized for doctor's notes data storage.

### System Architecture
The project architecture follows a microservices-based approach, enabling modular development, easy scalability, and efficient resource utilization. Each microservice is encapsulated within a Docker container to ensure portability and flexibility in deployment.

### Sprint 1
#### Tasks
Sprint 1 focuses on essential groundwork and infrastructure development:
- Docker environment setup.
- REST service for patient data access.

#### REST Service
The REST service provides access to patient records and is designed around the principles of RESTful architecture. It offers endpoints for data retrieval and modification.

#### Docker Infrastructure
The Docker infrastructure streamlines the deployment of microservices and ensures a consistent runtime environment across different stages of the project.

### Sprint 2
#### Tasks
Sprint 2 involves the development of the report generation feature, expanding the project's functionality.

#### Report Generation
Report generation is a critical component of the project, allowing healthcare professionals to analyze patient data and make informed decisions. This feature compiles patient information and produces comprehensive diabetes reports.

### Sprint 3
#### Tasks
Sprint 3 finalizes the project with integration and testing of all microservices.

#### Integration
This sprint involves the integration of all microservices to ensure that they work cohesively, contributing to the overall goal of diabetes prediction.

#### Test report
The test report is a comprehensive document that outlines the testing process and results. It includes a summary of the testing approach, test cases, and test results.
<img src="../MediScreenApp/ReadmeImages/tests_report.png" alt="Test Report" width="800"/>

## Conclusion
The Diabetes Prediction Project represents a significant step toward enhancing medical decision-making through technology. By leveraging microservices, Docker, and RESTful architecture, we have developed a versatile system that can contribute to the prediction of diabetes based on patient data. This README serves as both documentation and a guide for stakeholders, providing insight into the project's objectives, structure, and technical aspects.