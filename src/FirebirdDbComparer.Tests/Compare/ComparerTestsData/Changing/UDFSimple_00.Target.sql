﻿DECLARE EXTERNAL FUNCTION DLLVERSION
    BIGINT
RETURNS CSTRING(255) FREE_IT
ENTRY_POINT 'DLLVersion' MODULE_NAME 'Fast';