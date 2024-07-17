export interface iExpense{
    expenseId: number;
    groupId: number;
    description: string;
    amount: number;
    paidBy: string;
    date: Date;
    splitAmong: string;
    individualAmount :number;
}
