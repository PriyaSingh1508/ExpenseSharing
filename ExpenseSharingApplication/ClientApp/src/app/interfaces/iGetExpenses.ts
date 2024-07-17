export interface iGetExpenses {
    id: number;
    groupId: number;
    description: string;
    amount: number;
    paidBy: string;
    date: Date;
    splitAmong: string;
    individualAmount: number;
    contributedBy: string;
    isSettled: boolean;
}
