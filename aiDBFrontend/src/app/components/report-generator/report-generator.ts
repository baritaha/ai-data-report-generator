import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReportService } from '../../services/report.service';
import { HttpEventType } from '@angular/common/http';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-report-generator',
  templateUrl: './report-generator.html',
  styleUrls: ['./report-generator.scss'],
  imports:[CommonModule]
})
export class ReportGenerator implements OnInit {
  reportForm: FormGroup;
  selectedFile: File | null = null;
  isGenerating = false;
  generatedReport: any = null;
  uploadProgress = 0;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private reportService: ReportService
  ) {
    this.reportForm = this.fb.group({
      prompt: ['', [Validators.required, Validators.minLength(10)]],
      reportType: ['summary', Validators.required]
    });
  }

  ngOnInit(): void {}

    getFormattedDate(): string {
    if (this.generatedReport && this.generatedReport.generatedAt) {
      return new Date(this.generatedReport.generatedAt).toLocaleString();
    }
    return new Date().toLocaleString();
  }
  onFileSelected(event: any): void {
    const file: File = event.target.files[0];
    if (file) {
      const allowedTypes = ['text/csv', 'application/vnd.ms-excel', 
        'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'];
      
      if (allowedTypes.includes(file.type)) {
        this.selectedFile = file;
        this.errorMessage = '';
      } else {
        this.errorMessage = 'Please select a CSV or Excel file';
        this.selectedFile = null;
      }
    }
  }

  generateReport(): void {
    if (this.reportForm.valid && this.selectedFile) {
      this.isGenerating = true;
      this.generatedReport = null;
      this.errorMessage = '';

      const formData = new FormData();
      formData.append('file', this.selectedFile);
      formData.append('prompt', this.reportForm.get('prompt')?.value);
      formData.append('reportType', this.reportForm.get('reportType')?.value);

      this.reportService.generateReport(formData).subscribe({
        next: (event: any) => {
          if (event.type === HttpEventType.UploadProgress) {
            this.uploadProgress = Math.round(100 * event.loaded / event.total);
          } else if (event.type === HttpEventType.Response) {
            this.generatedReport = event.body;
            this.isGenerating = false;
            this.uploadProgress = 0;
          }
        },
        error: (error) => {
          this.errorMessage = 'Error generating report: ' + error.message;
          this.isGenerating = false;
          this.uploadProgress = 0;
        }
      });
    } else {
      this.errorMessage = 'Please provide both a file and a prompt';
    }
  }

  downloadPdf(): void {
    if (this.generatedReport) {
      this.reportService.downloadPdf(this.generatedReport.id).subscribe(blob => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `report-${new Date().getTime()}.pdf`;
        link.click();
        window.URL.revokeObjectURL(url);
      });
    }
  }

  clearForm(): void {
    this.reportForm.reset({ reportType: 'summary' });
    this.selectedFile = null;
    this.generatedReport = null;
    this.errorMessage = '';
  }
}