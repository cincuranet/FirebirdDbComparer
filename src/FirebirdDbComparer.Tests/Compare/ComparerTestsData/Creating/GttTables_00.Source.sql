create global temporary table gtt_delete (i int) on commit delete rows;
create global temporary table gtt_preserve (i int) on commit preserve rows;