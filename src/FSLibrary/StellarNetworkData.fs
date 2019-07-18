// Copyright 2019 Stellar Development Foundation and contributors. Licensed
// under the Apache License, Version 2.0. See the COPYING file at the root
// of this distribution or at http://www.apache.org/licenses/LICENSE-2.0

module StellarNetworkData

open stellar_dotnet_sdk

open StellarCoreSet

let PubnetGetCommands = [
                        "core_live_001", "curl -sf http://history.stellar.org/prd/core-live/core_live_001/{0} -o {1}"
                        "core_live_002", "curl -sf http://history.stellar.org/prd/core-live/core_live_002/{0} -o {1}"
                        "core_live_003", "curl -sf http://history.stellar.org/prd/core-live/core_live_003/{0} -o {1}"
                        ] |> Map.ofList

let PubnetNodes = [
                  "core_live_001", KeyPair.FromAccountId("GCGB2S2KGYARPVIA37HYZXVRM2YZUEXA6S33ZU5BUDC6THSB62LZSTYH")
                  "core_live_002", KeyPair.FromAccountId("GCM6QMP3DLRPTAZW2UZPCPX2LF3SXWXKPMP3GKFZBDSF3QZGV2G5QSTK")
                  "core_live_003", KeyPair.FromAccountId("GABMKJM6I25XI4K7U6XWMULOUQIQ27BCTMLS6BYYSOWKTBUXVRJSXHYQ")
                  ] |> Map.ofList

let PubnetPeers = [
                  "core-live4.stellar.org"
                  "core-live5.stellar.org"
                  "core-live6.stellar.org"
                  ]

let PubnetCoreSet = { CoreSetOptions.Default with
                        quorumSet = Some([])
                        quorumSetKeys = PubnetNodes
                        historyGetCommands = PubnetGetCommands
                        peersDns = PubnetPeers
                        initialization = { CoreSetInitialization.Default with forceScp = false } }

let TestnetGetCommands = [
                         "core_testnet_001", "wget -q http://history.stellar.org/prd/core-testnet/core_testnet_001/{0} -O {1}"
                         "core_testnet_002", "wget -q http://history.stellar.org/prd/core-testnet/core_testnet_002/{0} -O {1}"
                         "core_testnet_003", "wget -q http://history.stellar.org/prd/core-testnet/core_testnet_003/{0} -O {1}"
                         ] |> Map.ofList

let TestnetNodes = [
                   "core_testnet_001", KeyPair.FromAccountId("GDKXE2OZMJIPOSLNA6N6F2BVCI3O777I2OOC4BV7VOYUEHYX7RTRYA7Y")
                   "core_testnet_002", KeyPair.FromAccountId("GCUCJTIYXSOXKBSNFGNFWW5MUQ54HKRPGJUTQFJ5RQXZXNOLNXYDHRAP")
                   "core_testnet_003", KeyPair.FromAccountId("GC2V2EFSXN6SQTWVYA5EPJPBWWIMSD2XQNKUOHGEKB535AQE2I6IXV2Z")
                   ] |> Map.ofList

let TestnetPeers = [
                   "core-testnet1.stellar.org"
                   "core-testnet2.stellar.org"
                   "core-testnet3.stellar.org"
                   ]

let TestnetCoreSet = { CoreSetOptions.Default with
                         quorumSet = Some([])
                         quorumSetKeys = TestnetNodes
                         historyGetCommands = TestnetGetCommands
                         peersDns = TestnetPeers
                         initialization = { CoreSetInitialization.Default with forceScp = false } }