#!/usr/bin/env bash
sqlite3 /tmp/cheepDatabase.db < data/schema.sql
sqlite3 /tmp/cheepDatabase.db < data/dump.sql