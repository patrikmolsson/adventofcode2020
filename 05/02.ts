const fs = require('fs');


(function main() {
    const cards = inputToLines();

    const seatIds = new Set<number>(cards.map(getSeatId));

    const lowestSeatId = Array.from(seatIds).reduce((acc, curr) => Math.min(acc, curr), Number.MAX_VALUE);

    let currentSeatId = lowestSeatId + 1;
    while (true) {
        if (!seatIds.has(currentSeatId) && seatIds.has(currentSeatId - 1) && seatIds.has (currentSeatId + 1)) {
            console.log('match!', currentSeatId);
            break;
        }

        currentSeatId += 1;
    }

})();


function getSeatId(boardingCard: string): number {
    let rowLower = 0;
    let rowHigher = 127;
    let colLower = 0;
    let colHigher = 7;

    for (let i = 0; i < boardingCard.length; i += 1) {
        const char = boardingCard.charAt(i);

        switch (char) {
            case 'F': {
                rowHigher = rowHigher - Math.ceil((rowHigher - rowLower) / 2);
                break;
            }
            case 'B': {
                rowLower = rowLower + Math.ceil((rowHigher - rowLower) / 2);
                break;
            }
            case 'L': {
                colHigher = colHigher - Math.ceil((colHigher - colLower) / 2);
                break;
            }
            case 'R': {
                colLower = colLower + Math.ceil((colHigher - colLower) / 2);
                break;
            }
        }
    }

    // console.log(colLower, rowLower);

    if (rowHigher !== rowLower || colHigher !== colLower) {
        throw new Error('Uh oh');
    }

    return rowLower * 8 + colLower;
}
function inputToLines() {
    const lines = fs.readFileSync('./input.txt').toString('utf-8');

    return lines.split('\r\n');
}