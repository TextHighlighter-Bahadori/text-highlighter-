#!/bin/sh

#removing microservice containers
docker compose -f ../src/services/ClojureHighlighter/compose.yaml down
docker compose -f ../src/services/FortranHighlighter/compose.yaml down
docker compose -f ../src/services/CsharpHighlighter/compose.yaml down

#removing api gateway container
docker compose -f ../src/api-gateway/ApiGateway/compose.yaml down

#removing client application container
docker compose -f ../src/web/compose.yaml down

#removing observability infrastructures containers
docker compose -f ../src/observabiity-infrastructure/compose.yaml down
