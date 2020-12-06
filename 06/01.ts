const fs = require('fs');


(function main() {
    const groups = inputToGroupAnswers();

    let sum = 0;
    for (const group of groups) {
        const set = new Set([...group.split('')]);

        sum += set.size;
    }

    console.log(sum);
})();

function inputToGroupAnswers() {
    const lines = fs.readFileSync('./input.txt').toString('utf-8');

    return lines.split('\r\n\r\n').map(p => p.replace(/\r\n/g, ''), '');
}