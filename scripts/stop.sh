#!/bin/sh

#stoping microservices
docker compose -f ../src/services/ClojureHighlighter/compose.yaml stop
docker compose -f ../src/services/FortranHighlighter/compose.yaml stop
docker compose -f ../src/services/CsharpHighlighter/compose.yaml stop

#stoping api gatway
docker compose -f ../src/api-gateway/ApiGateway/compose.yaml stop

#stoping client application
docker compose -f ../src/web/compose.yaml stop

#stoping observability infrastructures
docker compose -f ../src/observabiity-infrastructure/compose.yaml stop
