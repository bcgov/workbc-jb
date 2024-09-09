select LockoutEnd,* from AspNetUsers

ALTER TABLE AspNetUsers
ALTER COLUMN LockoutEnd datetime;
