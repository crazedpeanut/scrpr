ARG SCRPR_VERSION=0.0.1
ARG SCRPR_TOPOLOGY=jk.storm.scrpr.topologies.WebCollectorTopology

FROM openjdk:13-alpine as build

ARG SCRPR_VERSION

ENV SCRPR_TOPOLOGY=${SCRPR_TOPOLOGY}

RUN apk add maven

COPY pom.xml .
COPY src ../src

RUN mvn package -Dscrpr.version=$SCRPR_VERSION

FROM storm:2.1.0 as release

ARG SCRPR_VERSION
ARG SCRPR_TOPOLOGY

ENV SCRPR_VERSION=${SCRPR_VERSION}
ENV SCRPR_TOPOLOGY=${SCRPR_TOPOLOGY}

COPY scripts/entrypoint.sh .

RUN chmod u+x entrypoint.sh

COPY --from=build target/scrpr-collectors-$SCRPR_VERSION.jar .
COPY storm.yaml /conf/storm.yaml

ENTRYPOINT [ "./entrypoint.sh" ]