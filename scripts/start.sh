#!/bin/sh

#starting microservices
docker compose -f ../src/services/ClojureHighlighter/compose.yaml start
docker compose -f ../src/services/FortranHighlighter/compose.yaml start
docker compose -f ../src/services/CsharpHighlighter/compose.yaml start

#starting api gatway
docker compose -f ../src/api-gateway/ApiGateway/compose.yaml start

#starting client application
docker compose -f ../src/web/compose.yaml start

#starting observability infrastructures
docker compose -f ../src/observabiity-infrastructure/compose.yaml start
