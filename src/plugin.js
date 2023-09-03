
export default class MyPlugin {

    afterStartup(_interpreter) {
        console.log("MyPlugin")
        // const target = document.querySelector("#target")
        // target.innerHTML = "<py-repl src=\"test.py\"></py-repl>"
    }
}