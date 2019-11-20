# QConsoleWeb
Web version of light control panel of roles and tables with geodata in database DBMS Postgis.

- View active sessions list;
- Manage roles and groups;
- Manage layers and dictionaries with the ability to enable/disable logging;
- Manage of access rights to layers and dictionaries;
- Getting statistics in graphs;
- View logger list with detailed information about changed attributes, operation, username, datetime, etc.
- Execute select-statements;
- Quick editing preset config files (for example, pg_hba).

To work, it requires pre-installed tables, functions, and triggers for logging tables.
https://github.com/johnzet39/LoggingPostgreSQL

First of all, the application was created to administer the GIS database and manage its geospatial layers and its users.
This allows a specialist unfamiliar with the concept of dbms to manage the database here and now. And to get some statistics, logging history of changes of attributes and features geometry.
At the moment, the limitation of using this application is that it is to some extent tied to a specific database structure:
- name of schema logger: "logger";
- name of table logger: "logtable";
- dictionaries (non-spatial tables) can be placed in the schema named "schema_spr". Also list of dictionaries can be posted in the table "logger"."dictionaries";
