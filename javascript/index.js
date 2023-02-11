require("dotenv").config();
const axios = require("axios").default;
const crypto = require("crypto");
const WebSocket = require("ws");

const ws = new WebSocket(process.env.STREAM_URL + "btcusdt@bookTicker");
let isOpened = false;

ws.onmessage = async (event) => {
    const obj = JSON.parse(event.data);
    console.log("Symbol: " + obj.s);
    console.log("Price: " + obj.a);

    const price = parseFloat(obj.a);
    if (price < 22980 && !isOpened) {
        console.log("Comprar!");
        
        isOpened = true;
    }
    else if (price > 23000 && isOpened) {
        console.log("Vender!");
     
        isOpened = false;
    }
}

