### Environment Setup:

####  Elastic Search:
#
```
1. docker pull docker.elastic.co/elasticsearch/elasticsearch:7.17.2
2. docker run --name es01-test --net elastic -p 9200:9200 -p 9300:9300 -e "discovery.type=single-node" docker.elastic.co/elasticsearch/elasticsearch:7.17.2
3. docker pull docker.elastic.co/kibana/kibana:7.17.2
4. docker run --name kib01-test --net elastic -p 5601:5601 -e "ELASTICSEARCH_HOSTS=http://es01-test:9200" docker.elastic.co/kibana/kibana:7.17.2
```
#

### Timings:
```
It takes approx. 1 minute 6 seconds to push all ~1M records to Elastic Search. 
Memory allocation: 1.5 GB
```


### Other Notes:
```
** I use 192.168.0.77:9200 instead of 127.0.0.1:9200 due to limitation of my local environment so basically elastic hosted on another machine

** Found that some lines in collections file is too short so temp solution is just to skip them

** Need to add Logging
```


