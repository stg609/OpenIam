import React from 'react';

export type ErrorBoundaryProps = {
    error: any
};

export type ErrorBoundaryState = {
    hasError: boolean,
    error: any
};


class ErrorBoundary extends React.Component<ErrorBoundaryProps, ErrorBoundaryState> {
    constructor(props: ErrorBoundaryProps) {
        super(props);
        this.state = { hasError: false, error: props.error };
    }

    static getDerivedStateFromError(error) {
        // 更新 state 使下一次渲染能够显示降级后的 UI
        return { hasError: true, error };
    }

    componentDidCatch(error, errorInfo) {
        // 你同样可以将错误日志上报给服务器
        //logErrorToMyService(error, errorInfo);
    }

    render() {
        if (this.state.hasError || this.state.error) {
            // 你可以自定义降级后的 UI 并渲染
            return <h1>Something went wrong. {this.state.error.message}</h1>;
        }

        return this.props.children;
    }
}

export default ErrorBoundary;