const { contracts, utils, wallet } = window.zilPay;
const contract = contracts.at("$$contractAddr$$");

async function createTx()
{
    const tx = await contract.call(
        'mint',
        [
          {
            vname : "to",
            type : "ByStr20",
            value : "0x8254b2C9aCdf181d5d6796d63320fBb20D4Edd12"
          }
        ],
        {
            vname : "token_uri",
            type : "String",
            value : "https://ivefwfclqyyavklisqgz.supabase.co/storage/v1/object/public/nftstorage/collection_example/metadata/4"
        },
        true
      ).then(([tx, contract]) => /do something.../);
    
    return tx;    
}


return createTx();
