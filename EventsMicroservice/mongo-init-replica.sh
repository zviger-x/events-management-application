#!/bin/bash

REPLICA_SET=${MONGO_REPLICA_SET:-rs0}
HOSTNAME=${MONGO_HOSTNAME:-MongoServer}
PORT=${MONGO_PORT:-27017}

echo "Starting MongoDB with replica: $REPLICA_SET. Host: $HOSTNAME:$PORT"

# ��������� MongoDB � ������ ����������
mongod --replSet "$REPLICA_SET" --bind_ip_all &

# ���, ���� MongoDB ����������
sleep 10

echo "Initializing replica..."

# �������������� �������
mongosh --host "$HOSTNAME:$PORT" --eval "
rs.initiate({
  _id: '$REPLICA_SET',
  members: [{ _id: 0, host: '$HOSTNAME:$PORT' }]
});
"

# �� ��� ���������� �����������
tail -f /dev/null