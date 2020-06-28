/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal
{
    /// <summary>
    /// Device Session State.
    /// </summary>
    public enum SessionState
    {
        /// <summary>
        /// UnInitialize means the NRSDK has not been initialized.
        /// </summary>
        UnInitialize = 0,

        /// <summary>
        /// TRACKING means the object is being tracked and its state is valid.
        /// </summary>
        Tracking,

        /// <summary>
        /// LostTracking means that NRSDK has lost tracking, and will never resume tracking. 
        /// </summary>
        LostTracking
    }
}
