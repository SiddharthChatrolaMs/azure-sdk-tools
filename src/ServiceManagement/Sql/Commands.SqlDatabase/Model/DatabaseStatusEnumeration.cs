﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.WindowsAzure.Commands.SqlDatabase
{
    /// <summary>
    /// Values that specify the status of a database, whether it be loading,
    /// recovering, or normal, for example.
    /// </summary>
    public enum DatabaseStatus
    {
        /// <summary>
        /// Referenced database is available for use (Online).
        /// </summary>
        Normal = 0x000001,

        /// <summary>
        /// Database restore is underway on the referenced database.
        /// </summary>
        Restoring = 0x000002,

        /// <summary>
        /// Database recovery is being prepared for the referenced database.
        /// </summary>
        RecoveryPending = 0x000004,

        /// <summary>
        /// Database recovery is underway on the referenced database.
        /// </summary>
        Recovering = 0x000008,

        /// <summary>
        /// Database integrity is suspect for the referenced database.
        /// </summary>
        Suspect = 0x000010,

        /// <summary>
        /// Referenced database has been placed offline by a system or user action.
        /// </summary>
        Offline = 0x000020,

        /// <summary>
        /// Referenced database defined on a standby server.
        /// </summary>
        Standby = 0x000040,

        /// <summary>
        /// Database is in Shutdown
        /// </summary>
        Shutdown = 0x000080,

        /// <summary>
        /// Emergency mode has been initiated on the referenced database.
        /// </summary>
        EmergencyMode = 0x000100,

        /// <summary>
        /// The database has been autoclosed.
        /// </summary>
        AutoClosed = 0x000200,

        /// <summary>
        /// The database is being created as a copy of another database (SQL Azure only)
        /// </summary>
        Copying = 0x000400,

        /// <summary>
        /// The database is creating - premium database (SQL Azure only)
        /// </summary>
        Creating = 0x000800,

        /// <summary>
        /// Property value that may be used for bitwisee AND operation to determine accessibility
        /// of the database (Restoring | Offline | Suspect | Recovering | RecoveryPending).
        /// </summary>
        Inaccessible = Restoring | Offline | Suspect | Recovering | RecoveryPending | Copying | Creating
    }
}
