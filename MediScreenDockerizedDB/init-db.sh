#!/bin/bash
# (see https://github.com/microsoft/mssql-docker/issues/2 )
echo "Container initialization: waiting for the server to come up"
while [ ! -f /var/opt/mssql/log/errorlog ]
do
  sleep 1
done
FOUND=0
i=0
while [[ $FOUND -ne 1 ]] && [[ $i -lt 60 ]]; do
  i=$i+1
  FOUND=$(grep -cim1 "Service Broker manager has started" /var/opt/mssql/log/errorlog)
  if [[ $FOUND -ne 1 ]]; then
    sleep 1
  fi
done
if [[ $FOUND -ne 1 ]]; then
  echo "Container initialization: Error: waited for more than 60 seconds for the server to start. Trying to create the database now..."
fi
sleep 5
echo "Container initialization: creating the database if needed"
/opt/mssql/bin/mssql-conf set sqlagent.enabled true

# Move the database files to the data directory
mv /app/CH.mdf /var/opt/mssql/data/
mv /app/CH_log.ldf /var/opt/mssql/data/
mv /app/CH_tests.mdf /var/opt/mssql/data/
mv /app/CH_tests_log.ldf /var/opt/mssql/data/

/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -Q "CREATE DATABASE CalifornianHealthDatabase ON (FILENAME = '/var/opt/mssql/data/CH.mdf'), (FILENAME = '/var/opt/mssql/data/CH_log.ldf') FOR ATTACH"
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -Q "CREATE DATABASE CalifornianHealthDatabaseTests ON (FILENAME = '/var/opt/mssql/data/CH_tests.mdf'), (FILENAME = '/var/opt/mssql/data/CH_tests_log.ldf') FOR ATTACH"
echo "Container initialization: done"

# Keep the script running (container won't exit)
tail -f /dev/null
