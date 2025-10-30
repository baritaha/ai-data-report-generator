import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface GeneratedReport {
  id: string;
  title: string;
  content: string;
  generatedAt: string;
  prompt: string;
  reportType: string;
  fileName: string;
}

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  private apiUrl = 'http://localhost:5138/api';

  constructor(private http: HttpClient) { }

  generateReport(formData: FormData): Observable<any> {
    return this.http.post(`${this.apiUrl}/reports/generate`, formData, {
      reportProgress: true,
      observe: 'events'
    });
  }

  downloadPdf(reportId: string): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/reports/${reportId}/download`, {
      responseType: 'blob'
    });
  }

  getReportHistory(): Observable<GeneratedReport[]> {
    return this.http.get<GeneratedReport[]>(`${this.apiUrl}/reports/history`);
  }

  getReportById(id: string): Observable<GeneratedReport> {
    return this.http.get<GeneratedReport>(`${this.apiUrl}/reports/${id}`);
  }
}