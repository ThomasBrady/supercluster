// Copyright 2019 Stellar Development Foundation and contributors. Licensed
// under the Apache License, Version 2.0. See the COPYING file at the root
// of this distribution or at http://www.apache.org/licenses/LICENSE-2.0

module MissionHistoryPubnetCompleteCatchup

open StellarCoreSet
open StellarMissionContext
open StellarNetworkCfg
open StellarNetworkData
open StellarSupercluster

let historyPubnetCompleteCatchup (context : MissionContext) =
    let set = { PubnetCoreSet with nodeCount = 1; catchupMode = CatchupComplete }
    let coreSet = MakeLiveCoreSet "core" set
    context.Execute [coreSet] (Some(SDFMainNet)) (fun (formation: ClusterFormation) ->
        formation.WaitUntilSynced [coreSet]
    )