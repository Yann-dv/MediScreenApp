# Choose exact tag (not 'latest'), to be sure that new version will not break creating image
FROM mcr.microsoft.com/mssql/server:2019-latest

# Set environment variables, not to have to write them with docker run command
ENV SA_PASSWORD Password+123
ENV ACCEPT_EULA Y
ENV MSSQL_PID Developer

# Set the working directory
WORKDIR /app

# Copy initialization scripts and database files
# Remove MediScreenDockerizedDB for docker-compose without others services
COPY MediScreenDockerizedDB/entrypoint.sh ./
COPY MediScreenDockerizedDB/init-db.sh ./
COPY MediScreenDockerizedDB/MediScreenDb.mdf ./
COPY MediScreenDockerizedDB/MediScreenDb_log.ldf ./

USER root 
RUN chmod +x entrypoint.sh init-db.sh

# Run Microsoft SQL Server and initialization script (at the same time)
CMD /bin/bash ./entrypoint.sh
